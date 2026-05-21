using EasyApply.BusinessLayer.Structure.DTOs.Job;
using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApply.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    // GET: api/job/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _jobService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/job/company/{companyId}?activeOnly=true
    [HttpGet("company/{companyId:guid}")]
    public async Task<IActionResult> GetByCompanyId(
        Guid companyId,
        [FromQuery] bool activeOnly = true)
    {
        var result = await _jobService.GetByCompanyIdAsync(companyId, activeOnly);
        return Ok(result);
    }

    // GET: api/job/search?keyword=...&location=...&page=1&pageSize=10
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? keyword,
        [FromQuery] string? location,
        [FromQuery] string? category,
        [FromQuery] string? employmentType,
        [FromQuery] string? experienceLevel,
        [FromQuery] int? locationType,
        [FromQuery] decimal? minSalary,
        [FromQuery] decimal? maxSalary,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var (jobs, total) = await _jobService.SearchAsync(
            keyword, location, category, employmentType, experienceLevel,
            locationType, minSalary, maxSalary, page, pageSize);
        return Ok(new { Jobs = jobs, Total = total, Page = page, PageSize = pageSize });
    }

    // POST: api/job/search
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchJobDto searchDto)
    {
        var result = await _jobService.SearchAsync(searchDto);
        return Ok(result);
    }

    // GET: api/job/nearby?lat=47.01&lng=28.86&radiusKm=10
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radiusKm = 10)
    {
        if (radiusKm <= 0 || radiusKm > 500)
            return BadRequest("radiusKm must be between 1 and 500.");

        var result = await _jobService.GetNearbyAsync(lat, lng, radiusKm);
        return Ok(result);
    }

    // POST: api/job
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto)
    {
        var result = await _jobService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT: api/job/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobDto dto)
    {
        var result = await _jobService.UpdateAsync(id, dto);
        return Ok(result);
    }

    // DELETE: api/job/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _jobService.DeleteAsync(id);
        return NoContent();
    }

    // POST: api/job/{id}/view
    [HttpPost("{id:guid}/view")]
    public async Task<IActionResult> IncrementViewCount(Guid id)
    {
        await _jobService.IncrementViewCountAsync(id);
        return NoContent();
    }

    // GET: api/job/recommendations/{id}?count=5
    [HttpGet("recommendations/{id:guid}")]
    public async Task<IActionResult> GetRecommendations(Guid id, [FromQuery] int count = 5)
    {
        var result = await _jobService.GetRecommendationsAsync(id, count);
        return Ok(result);
    }

    // GET: api/job/benchmark-salary?category=...&experienceLevel=...&salaryMin=...&salaryMax=...
    [HttpGet("benchmark-salary")]
    public async Task<IActionResult> GetSalaryBenchmark([FromQuery] SalaryBenchmarkRequest request)
    {
        var result = await _jobService.GetSalaryBenchmarkAsync(request);
        return Ok(result);
    }
}