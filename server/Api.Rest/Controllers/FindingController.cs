using Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;


[ApiController]
public class FindingController(IFindingService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Finding>> GetFindingWithId([FromBody] Guid id)
    {
        return await service.GetFindingWithId(id);
    }

    [HttpPost]
    public async Task<ActionResult<bool>> AddFinding([FromBody] Finding finding)
    { 
        await service.AddFinding(finding);
        return Created();
    }

    [HttpPatch]
    public async Task<ActionResult<bool>> UpdateFinding([FromBody] Finding finding)
    {
        await service.UpdateFinding(finding);
        return Ok();
    }
}