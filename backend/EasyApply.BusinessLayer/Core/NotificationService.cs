using EasyApply.BusinessLayer.Structure.DTOs.Notification;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;

namespace EasyApply.BusinessLayer.Core;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Link = n.Link,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        });
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task CreateNotificationAsync(Guid userId, string title, string message, string? link = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Link = link,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();
    }
}
