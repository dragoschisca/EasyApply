using EasyApply.Domains.Entities;
using EasyApply.Domains.Interfaces.Repositories;

namespace EasyApply.Domains.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
   // Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
