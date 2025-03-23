using System.Text.Json;
using Domain;
using MediatR;

namespace Application.BusinessLogic.Queries.GetLetterCountsList;

public class GetLetterCountsListQuery : IRequest<JsonDocument>
{
    public string UserId { get; set; }
    public string AccessToken { get; set; }
}