using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.BusinessLogic.Queries.GetLetterCountsList;
using Application.Common.Exceptions;
using Application.Interfaces;
using Domain;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Persistence;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class Tests
{
    private IHttpClientFactory _httpClientFactory;
    private IAppDbContext _dbContext;
    private static string? _accessToken;
    private GetLetterCountsListValidator _validator;

    
    [SetUp]
    public void OneTimeSetup()
    {
        _dbContext = MockAppDbContext.Create();
        
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient());
        _httpClientFactory = httpClientFactoryMock.Object;
        
        _accessToken = EnvironmentVariables.AccessToken;
        
        _validator = new GetLetterCountsListValidator();
    }


    [TearDown]
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

        Assert.That(result == MockAppDbContext.Data.First().Value, Is.True);
        
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<JsonDocument>());

        
        var propertyNames = result.RootElement.EnumerateObject();
        var propertyNamesList = propertyNames.Select(property => property.Name).ToList();
        
        Assert.That(propertyNames.ToList(), Is.Not.Empty);

        foreach (var property in propertyNames)
        {
            Assert.Multiple(() =>
            {
                Assert.That(property.Name.Length == 1 && char.IsLetter(property.Name[0]));

                Assert.That(property.Value.ValueKind == JsonValueKind.Number && property.Value.TryGetInt32(out _));
            });
        }

        Assert.That(propertyNamesList
            .Zip(propertyNamesList.Skip(1), (current, next) => String.CompareOrdinal(current, next) <= 0)
            .All(isOrdered => isOrdered), Is.True);
        
        Assert.That(_accessToken == MockAppDbContext.Data.First().GetDecryptedAccessToken(), Is.True);
    }
    
    [Test]
    public async Task Handle_ValidRequest_ReturnsLetterCounts2()
    {
        var handler = new GetLetterCountsListQueryHandler(_httpClientFactory, _dbContext);
        var result1 = await handler.Handle(
            new GetLetterCountsListQuery
            {
                UserId = "botay_suka",
                AccessToken = _accessToken,
            }, CancellationToken.None);
        
        var result2 = await handler.Handle(
            new GetLetterCountsListQuery
            {
                UserId = "botay_suka",
                AccessToken = _accessToken,
            }, CancellationToken.None);

        Assert.That(result1.ToString() == result2.ToString(), Is.True);
        Assert.That(MockAppDbContext.Data[0].Id != MockAppDbContext.Data[1].Id, Is.True);
        Assert.That(MockAppDbContext.Data[0].AccessToken == MockAppDbContext.Data[1].AccessToken, Is.True);
        
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