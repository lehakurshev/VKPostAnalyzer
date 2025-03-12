using Application.BusinessLogic.Queries.GetLetterCountsList;
using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace Tests;

public class Tests : IDisposable, IAsyncDisposable
{
    private IHttpClientFactory _httpClientFactory;
    private AppDbContext _dbContext { get; set; }
    private string? _accessToken;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();
        _dbContext.SaveChanges();
        
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient());
        _httpClientFactory = httpClientFactoryMock.Object;
        
        DotNetEnv.Env.TraversePath().Load();
        _accessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
    }

    [Test]
    public async Task Handle_ValidRequest_ReturnsLetterCounts()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);
        var result = await handler.Handle(
            new GetLetterCountsListQuery
            {
                UserId = "botay_suka",
                AccessToken = _accessToken,
            }, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.IsNotEmpty(result);
        Assert.IsTrue(result.Count > 0);
        
        var sortedResult = result.OrderBy(letterCount =>
            letterCount.Letter, Comparer<string>.Create((x, y) =>
            string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase)
        )!).ToList();
        Assert.That(result, Is.EqualTo(sortedResult));

    }
    
    [Test]
    public async Task Handle_InvalidAccessToken_ThrowsInvalidAccessTokenException()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);
        var ex = Assert.ThrowsAsync<InvalidAccessTokenException>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "botay_suka",
                    AccessToken = "bad_access_token"
                }, CancellationToken.None);
        });
    }
    
    [Test]
    public async Task Handle_UserHasHiddenWall_ThrowsUserHidWallException()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);

        Assert.ThrowsAsync<UserHidWallException>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "bad_id",
                    AccessToken = _accessToken,
                }, CancellationToken.None);
        });
    }
    
    [Test]
    public async Task Handle_InvalidUserIdFormat_ThrowsInvalidIdException()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);

        Assert.ThrowsAsync<InvalidIdException>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "bad_id_bad_id_bad_id",
                    AccessToken = _accessToken,
                }, CancellationToken.None);
        });
    }
    
    [Test]
    public async Task Handle_GeneralExceptionFromExternalService_ThrowsException()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);

        Assert.ThrowsAsync<Exception>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "rdtvs",
                    AccessToken = _accessToken,
                }, CancellationToken.None);
        });
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
       await _dbContext.DisposeAsync();
    }
}