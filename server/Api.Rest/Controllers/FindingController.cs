using Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;


[Route(ControllerRoute)]
[ApiController]
public class FindingController(IFindingService service) : ControllerBase
{
    public const string ControllerRoute = "api/finding/";
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Finding>> GetFindingWithId([FromRoute] Guid id)
    {
        var result = await service.GetFindingWithId(id);
        if (result == null)
        {
            return BadRequest("The finding your looking for doesn't exist");
        }
        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<ActionResult<bool>> AddFinding([FromBody] Finding finding)
    { 
        await service.AddFinding(finding);
        return Created();
    }

    [HttpPatch("update")]
    public async Task<ActionResult<bool>> UpdateFinding([FromBody] Finding finding)
    {
        await service.UpdateFinding(finding);
        return Ok();
    }
}