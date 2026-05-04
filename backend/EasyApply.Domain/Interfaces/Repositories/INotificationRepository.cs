using EasyApply.Domain.Entities;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
}
