using EasyApply.Domain.Entities;
using EasyApply.Domain.Models.Interfaces.Repositories;

namespace EasyApply.Domain.Models.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
   // Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
