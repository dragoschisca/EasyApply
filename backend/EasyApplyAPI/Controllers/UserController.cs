using EasyApply.Application.DTOs.User;
using EasyApply.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApplyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/user/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Ok(result);
    }

    // GET: api/user/email/{email}
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var result = await _userService.GetByEmailAsync(email);
        return Ok(result);
    }

    // GET: api/user
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result);
    }
    
    // POST: api/user
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // PUT: api/user/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        return Ok(result);
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}