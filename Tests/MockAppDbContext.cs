using System.Text.Json;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain;
using Moq;

namespace Tests;


public static class MockAppDbContext
{
    public static List<LetterCountRequestData>? Data;
    
    public static IAppDbContext Create()
    {
        var mockRepository = new MockRepository(MockBehavior.Default);
        var mockContext = mockRepository.Create<IAppDbContext>();
        mockContext.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.FromResult(int.MaxValue));
        mockContext.Setup(db => db.LetterCountRequestData).Returns(MockDbSet().Object);
        return mockContext.Object;
    }
    
    private static Mock<DbSet<LetterCountRequestData>> MockDbSet()
    {
        Data = new List<LetterCountRequestData>();
        
        var queryable = Data.AsQueryable();

        var mock = new Mock<DbSet<LetterCountRequestData>>();

        mock.As<IQueryable<LetterCountRequestData>>().Setup(m => m.Provider)
            .Returns(queryable.Provider);
        mock.As<IQueryable<LetterCountRequestData>>().Setup(m => m.Expression)
            .Returns(queryable.Expression);
        mock.As<IQueryable<LetterCountRequestData>>().Setup(m => m.ElementType)
            .Returns(queryable.ElementType);
        mock.As<IQueryable<LetterCountRequestData>>().Setup(m => m.GetEnumerator())
            .Returns(queryable.GetEnumerator());

        mock.Setup(m => m.AddAsync(It.IsAny<LetterCountRequestData>(), It.IsAny<CancellationToken>()))
            .Callback((LetterCountRequestData entity, CancellationToken token) =>
            {
                Data.Add(entity);
            });

        return mock;
    }
}
