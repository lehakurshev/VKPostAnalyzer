using Application.BusinessLogic.Queries.GetLetterCountsList;
using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Tests;

public class Tests : IDisposable, IAsyncDisposable
{
    private IHttpClientFactory _httpClientFactory;
    private AppDbContext _dbContext { get; set; }
    private string? _accessToken;
    private Mock<IValidator<GetLetterCountsListQuery>> _mockValidator = new Mock<IValidator<GetLetterCountsListQuery>>();
    
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