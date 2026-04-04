using EasyApply.BusinessLayer.Structure.DTOs.CV;
using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApply.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class CVController : ControllerBase
{
    private readonly ICVService _cvService;

    public CVController(ICVService cvService)
    {
        _cvService = cvService;
    }

    // GET: api/cv/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _cvService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/cv/candidate/{candidateId}
    [HttpGet("candidate/{candidateId:guid}")]
    public async Task<IActionResult> GetByCandidateId(Guid candidateId)
    {
        var result = await _cvService.GetByCandidateIdAsync(candidateId);
        return Ok(result);
    }

    // GET: api/cv/candidate/{candidateId}/primary
    [HttpGet("candidate/{candidateId:guid}/primary")]
    public async Task<IActionResult> GetPrimaryByCandidateId(Guid candidateId)
    {
        var result = await _cvService.GetPrimaryByCandidateIdAsync(candidateId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // POST: api/cv/{candidateId}
    [HttpPost("{candidateId:guid}")]
    public async Task<IActionResult> Create(Guid candidateId, [FromBody] CreateCVDto dto)
    {
        var result = await _cvService.CreateAsync(candidateId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT: api/cv/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCVDto dto)
    {
        var result = await _cvService.UpdateAsync(id, dto);
        return Ok(result);
    }

    // DELETE: api/cv/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _cvService.DeleteAsync(id);
        return NoContent();
    }

    // PATCH: api/cv/{id}/primary/{candidateId}
    [HttpPatch("{id:guid}/primary/{candidateId:guid}")]
    public async Task<IActionResult> SetPrimary(Guid id, Guid candidateId)
    {
        await _cvService.SetPrimaryAsync(id, candidateId);
        return NoContent();
    }
}
