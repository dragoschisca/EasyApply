using EasyApply.BusinessLayer.Structure.DTOs.Notification;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(Guid userId);
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync(Guid userId);
    Task CreateNotificationAsync(Guid userId, string title, string message, string? link = null);
}
