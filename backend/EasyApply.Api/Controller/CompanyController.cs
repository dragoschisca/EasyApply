using EasyApply.BusinessLayer.Structure.DTOs.Company;
using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApply.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    // GET: api/company/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _companyService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/company/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _companyService.GetByUserIdAsync(userId);
        return Ok(result);
    }

    // GET: api/company?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _companyService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    // POST: api/company/{userId}
    [HttpPost("{userId:guid}")]
    public async Task<IActionResult> Create(Guid userId, [FromBody] CreateCompanyDto dto)
    {
        var result = await _companyService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT: api/company/{userId}
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateCompanyDto dto)
    {
        var result = await _companyService.UpdateAsync(userId, dto);
        return Ok(result);
    }

    // DELETE: api/company/{userId}
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId)
    {
        await _companyService.DeleteAsync(userId);
        return NoContent();
    }
}
