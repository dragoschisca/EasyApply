using EasyApply.BusinessLayer.Structure.DTOs.Notification;
using EasyApply.BusinessLayer.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyApply.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var result = await _notificationService.GetByUserIdAsync(userId);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpPatch("user/{userId:guid}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(Guid userId)
    {
        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}
