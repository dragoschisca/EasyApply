using EasyApply.Application.DTOs.SavedJob;
using EasyApply.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApplyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavedJobController : ControllerBase
{
    private readonly ISavedJobService _savedJobService;

    public SavedJobController(ISavedJobService savedJobService)
    {
        _savedJobService = savedJobService;
    }

    // GET: api/savedjob/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _savedJobService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/savedjob/candidate/{candidateId}
    [HttpGet("candidate/{candidateId:guid}")]
    public async Task<IActionResult> GetByCandidateId(Guid candidateId)
    {
        var result = await _savedJobService.GetByCandidateIdAsync(candidateId);
        return Ok(result);
    }

    // POST: api/savedjob
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSavedJobDto dto)
    {
        var result = await _savedJobService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // DELETE: api/savedjob/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _savedJobService.DeleteAsync(id);
        return NoContent();
    }
}
