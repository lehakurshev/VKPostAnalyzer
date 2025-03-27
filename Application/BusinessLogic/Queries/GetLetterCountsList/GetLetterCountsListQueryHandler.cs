using System.Text.Json;
using Application.Common.ErrorCodes;
using Application.Common.Exceptions;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BusinessLogic.Queries.GetLetterCountsList;

public class GetLetterCountsListQueryHandler : IRequestHandler<GetLetterCountsListQuery, JsonDocument>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAppDbContext _dbContext;

    public GetLetterCountsListQueryHandler(IHttpClientFactory httpClientFactory, IAppDbContext dbContext)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
    }
    
    public async Task<JsonDocument> Handle(GetLetterCountsListQuery request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var url = $"https://api.vk.com/method/wall.get?owner_id=" +
                  $"{request.UserId}&count=5&access_token={request.AccessToken}&v=5.131";
        var response = await client.GetStringAsync(url, cancellationToken);
        
        var result = await ParsePostsAsync(response);

        await _dbContext.LetterCountRequestData.AddAsync(new LetterCountRequestData(
            request.UserId,
            result
        ), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }
    
    private static async Task<JsonDocument> ParsePostsAsync(string jsonResponse)
    {
        using var doc = JsonDocument.Parse(jsonResponse);
        var root = doc.RootElement;

        if (root.TryGetProperty("error", out var errorElement))
        {
            HandleErrorCode(errorElement);
        }

        if (!root.TryGetProperty("response", out var responseElement) ||
            !responseElement.TryGetProperty("items", out var itemsElement))
        {
            return JsonDocument.Parse("{}");
        }

        var letterCounts = new Dictionary<char, int>();
        
        var chars = itemsElement.EnumerateArray()
            .Where(item => item.TryGetProperty("text", out _))
            .Select(item => item.GetProperty("text").GetString()?.ToLower() ?? "")
            .SelectMany(text => text.Where(char.IsLetter));

        chars.GroupBy(c => c)
            .ToList()
            .ForEach(g => letterCounts.TryAdd(g.Key, g.Count()));

        var sortedLetterCounts = letterCounts.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonString = JsonSerializer.Serialize(sortedLetterCounts, options);
        return JsonDocument.Parse(jsonString);
    }

    private static void HandleErrorCode(JsonElement errorElement)
    {
        if (!errorElement.TryGetProperty("error_code", out var errorCodeElement))
            throw new VkApiException("Error in VK API response: " + errorElement.GetRawText());

        var errorCode = errorCodeElement.GetInt32();

        throw errorCode switch
        {
            VkApiErrorCodes.InvalidAccessToken => new InvalidAccessTokenException("Invalid access token provided."),
            VkApiErrorCodes.UserHidWall => new UserHidWallException("The user has hidden their wall."),
            VkApiErrorCodes.InvalidUserId => new InvalidIdException("Invalid user ID provided."),
            _ => new VkApiException("Error in VK API response: " + errorElement.GetRawText())
        };
    }
}