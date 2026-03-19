using EasyApply.Application.DTOs.User;

namespace EasyApply.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> GetByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
    Task DeleteAsync(Guid id);
}
