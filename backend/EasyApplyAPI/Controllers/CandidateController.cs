using global::EasyApply.Application.Interfaces.Services;
using global::EasyApply.Application.DTOs.Candidate;
using Microsoft.AspNetCore.Mvc;

namespace EasyApplyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    // GET: api/candidate/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _candidateService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/candidate/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _candidateService.GetByUserIdAsync(userId);
        return Ok(result);
    }

    // GET: api/candidate?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _candidateService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    // POST: api/candidate/{userId}
    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> Create(Guid userId, [FromBody] CreateCandidateDto dto)
    {
        var candidate = await _candidateService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = candidate.Id }, candidate);
    }

    // PUT: api/candidate/{userId}
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateCandidateDto dto)
    {
        var result = await _candidateService.UpdateAsync(userId, dto);
        return Ok(result);
    }

    // DELETE: api/candidate/{userId}
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId)
    {
        await _candidateService.DeleteAsync(userId);
        return NoContent();
    }

    // GET: api/candidate/search?term=alex
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _candidateService.SearchAsync(term);
        return Ok(result);
    }
}