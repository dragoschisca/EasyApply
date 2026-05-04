using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyApply.DataAccess.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification entity)
    {
        await _context.Notifications.AddAsync(entity);
    }

    public async Task UpdateAsync(Notification entity)
    {
        _context.Notifications.Update(entity);
    }

    public async Task DeleteAsync(Notification entity)
    {
        _context.Notifications.Remove(entity);
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications.ToListAsync();
    }

    public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetPagedAsync(int skip, int take)
    {
        var total = await _context.Notifications.CountAsync();
        var items = await _context.Notifications.OrderByDescending(n => n.CreatedAt).Skip(skip).Take(take).ToListAsync();
        return (items, total);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();
        
        foreach (var n in unread)
        {
            n.IsRead = true;
        }
        
        await _context.SaveChangesAsync();
    }
}
