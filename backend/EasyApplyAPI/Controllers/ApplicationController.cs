using EasyApply.Application.DTOs.Application;
using EasyApply.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApplyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // GET: api/application/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _applicationService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/application/candidate/{candidateId}
    [HttpGet("candidate/{candidateId:guid}")]
    public async Task<IActionResult> GetByCandidateId(Guid candidateId)
    {
        var result = await _applicationService.GetByCandidateIdAsync(candidateId);
        return Ok(result);
    }

    // GET: api/application/job/{jobId}
    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetByJobId(Guid jobId)
    {
        var result = await _applicationService.GetByJobIdAsync(jobId);
        return Ok(result);
    }

    // POST: api/application/{candidateId}
    [HttpPost("{candidateId:guid}")]
    public async Task<IActionResult> Create(Guid candidateId, [FromBody] CreateApplicationDto dto)
    {
        var result = await _applicationService.CreateAsync(candidateId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PATCH: api/application/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateApplicationStatusDto dto)
    {
        var result = await _applicationService.UpdateStatusAsync(id, dto);
        return Ok(result);
    }

    // DELETE: api/application/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _applicationService.DeleteAsync(id);
        return NoContent();
    }
}
