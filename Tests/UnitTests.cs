using System.ComponentModel.DataAnnotations;
using Application.BusinessLogic.Queries.GetLetterCountsList;
using Application.Common.Exceptions;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence;

namespace Tests;

[TestFixture]
public class Tests
{
    private IHttpClientFactory _httpClientFactory;
    private AppDbContext _dbContext;
    private static string? _accessToken;
    private GetLetterCountsListValidator _validator;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
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
        
        _validator = new GetLetterCountsListValidator();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _dbContext.Dispose();
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
    
    [TestCaseSource(nameof(ExceptionTestCases))]
    public async Task Handle_ExceptionCases_ThrowsExpectedException
        (string userId, string? accessToken, Type expectedExceptionType)
    {
        accessToken ??= _accessToken;
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);
        var query = new GetLetterCountsListQuery { UserId = userId, AccessToken = accessToken };

        Assert.ThrowsAsync(expectedExceptionType, async () =>
        {
            await handler.Handle(query, CancellationToken.None);
        });
    }
    
    private static readonly object[][] ExceptionTestCases =
    [
        ["botay_suka", "bad_access_token", typeof(InvalidAccessTokenException)],
        ["bad_id", null, typeof(UserHidWallException)],
        ["bad_id_bad_id_bad_id", null, typeof(InvalidIdException)],
        ["rdtvs", null, typeof(Exception)]
    ];
    
    [TestCaseSource(nameof(ValidationResultTestCases))]
    public async Task Validator_ShouldValidateCorrectly(string accessToken, string userId,
        Action<TestValidationResult<GetLetterCountsListQuery>> assertAction)
    {
        var query = new GetLetterCountsListQuery
        {
            AccessToken = accessToken,
            UserId = userId
        };
        var result = _validator.TestValidate(query);
        assertAction(result);
    }
    
    private static readonly object[][] ValidationResultTestCases =
    [
        [
            "ValidToken123", "ValidUser123", 
            (Action<TestValidationResult<GetLetterCountsListQuery>>)
            (result => result.ShouldNotHaveAnyValidationErrors())
        ],
        [
            "", "ValidUser123", 
            (Action<TestValidationResult<GetLetterCountsListQuery>>)
            (result => result.ShouldHaveValidationErrorFor(x => x.AccessToken))
        ],
        [
            "Invalid@Token!", "ValidUser123", 
            (Action<TestValidationResult<GetLetterCountsListQuery>>)
            (result => result.ShouldHaveValidationErrorFor(x => x.AccessToken))
        ],
        [
            "ValidToken123", "", 
            (Action<TestValidationResult<GetLetterCountsListQuery>>)
            (result => result.ShouldHaveValidationErrorFor(x => x.UserId))
        ],
        [
            "ValidToken123", "Invalid@User!", 
            (Action<TestValidationResult<GetLetterCountsListQuery>>)
            (result => result.ShouldHaveValidationErrorFor(x => x.UserId))
        ]
    ];
}