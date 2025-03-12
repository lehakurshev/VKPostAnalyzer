using Application.BusinessLogic.Queries.GetLetterCountsList;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace Tests;

public class Tests : IDisposable, IAsyncDisposable
{
    private IHttpClientFactory _httpClientFactory;
    private AppDbContext _dbContext { get; set; }
    
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
    }

    [Test]
    public async Task Test1()
    {
        var a = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);
        var result = await handler.Handle(
            new GetLetterCountsListQuery
            {
                UserId = "botay_suka",
                AccessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN"),
            }, CancellationToken.None);
        
        Assert.NotNull(result);
    }
    
    [Test]
    public async Task Test2()
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
        
        Assert.That(ex, Is.InstanceOf<InvalidAccessTokenException>());
    }
    
    [Test]
    public async Task Test3()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);

        var ex = Assert.ThrowsAsync<UserHidWallException>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "bad_id",
                    AccessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN"),
                }, CancellationToken.None);
        });

        Assert.That(ex, Is.InstanceOf<UserHidWallException>());
    }
    
    [Test]
    public async Task Test4()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);

        var ex = Assert.ThrowsAsync<InvalidIdException>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "bad_id_bad_id_bad_id",
                    AccessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN"),
                }, CancellationToken.None);
        });

        Assert.That(ex, Is.InstanceOf<InvalidIdException>());
    }
    
    [Test]
    public async Task Test5()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);

        var ex = Assert.ThrowsAsync<Exception>(async () =>
        {
            await handler.Handle(
                new GetLetterCountsListQuery
                {
                    UserId = "rdtvs",
                    AccessToken = Environment.GetEnvironmentVariable("ACCESS_TOKEN"),
                }, CancellationToken.None);
        });

        Assert.That(ex, Is.InstanceOf<Exception>());
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