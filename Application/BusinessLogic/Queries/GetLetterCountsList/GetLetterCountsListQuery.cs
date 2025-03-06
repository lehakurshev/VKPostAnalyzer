using Domain;
using MediatR;

namespace Application.BusinessLogic.Queries.GetLetterCountsList;

public class GetLetterCountsListQuery : IRequest<IList<LetterCount>>
{
    public string? UserId { get; set; }
    public string? AccessToken { get; set; }
}