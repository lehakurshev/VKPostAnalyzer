using System.Text.Json;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.Queries.GetLetterCountsList;

public class GetLetterCountsListQueryHandler : IRequestHandler<GetLetterCountsListQuery, IList<LetterCount>>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAppDbContext _dbContext;

    public GetLetterCountsListQueryHandler(IHttpClientFactory httpClientFactory, IAppDbContext dbContext)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
    }
    
    public async Task<IList<LetterCount>> Handle(GetLetterCountsListQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var url = $"https://api.vk.com/method/wall.get?owner_id={request.UserId}&count=5&access_token={request.AccessToken}&v=5.131";
        var response = await client.GetStringAsync(url, cancellationToken);

        var posts = await ParsePostsAsync(response);

        var letterCount = posts
            .SelectMany(post => post.ToLower().Where(char.IsLetter))
            .GroupBy(c => c)
            .Select(g => new LetterCount { Letter = g.Key.ToString(), Count = g.Count(), Id = Guid.NewGuid() })
            .OrderBy(lc => lc.Letter)
            .ToList();

        _dbContext.LetterCounts.RemoveRange(_dbContext.LetterCounts);
        await _dbContext.LetterCounts.AddRangeAsync(letterCount, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await _dbContext.LetterCounts.ToListAsync(cancellationToken);
    }
    
    private static async Task<List<string>> ParsePostsAsync(string jsonResponse)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("error", out _))
                {
                    throw new Exception("Error in VK API response: " + jsonResponse);
                }

                if (!root.TryGetProperty("response", out var responseElement) ||
                    !responseElement.TryGetProperty("items", out var itemsElement))
                {
                    return new List<string>();
                }

                return itemsElement.EnumerateArray()
                    .Where(item => item.TryGetProperty("text", out var textElement))
                    .Select(item => item.GetProperty("text").GetString())
                    .ToList()!;
            }
            catch (JsonException ex)
            {
                throw new Exception("Error parsing JSON: " + ex.Message + " - Response: " + jsonResponse);
            }
        });
    }
}