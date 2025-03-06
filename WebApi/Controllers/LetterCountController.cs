using Application.BusinessLogic.Queries.GetLetterCountsList;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LetterCountController : BaseController
{
    [HttpGet("{userId}/{accessToken}")]
    public async Task<ActionResult<List<LetterCount>>> GetPostAnalysis(string userId, string accessToken)
    {
        var query = new GetLetterCountsListQuery{ UserId = userId, AccessToken = accessToken };
        var counts = await Mediator.Send(query);
        return Ok(counts);
    }
}