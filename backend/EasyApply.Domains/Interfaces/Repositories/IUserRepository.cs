using EasyApply.Domains.Enums;
using EasyApply.Domains.Interfaces.Repositories;

namespace EasyApply.Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
