namespace EasyApply.Domains.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int skip, int take);

    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    Task SaveChangesAsync();
}