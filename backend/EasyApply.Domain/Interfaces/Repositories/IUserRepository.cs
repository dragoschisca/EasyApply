using EasyApply.Domain.Entities;
using EasyApply.Domain.Interfaces.Repositories;

namespace EasyApply.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
   // Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
