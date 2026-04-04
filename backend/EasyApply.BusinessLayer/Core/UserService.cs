using EasyApply.BusinessLayer.Structure.DTOs.User;
using EasyApply.Domain.Exceptions;
using EasyApply.Domain.Models.Interfaces.Repositories;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.Domain.Entities;

namespace EasyApply.BusinessLayer.Core;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new NotFoundException($"User with ID {id} not found.");
        return MapToDto(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) throw new NotFoundException($"User with email {email} not found.");
        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }
    
    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new Exception($"User with email {dto.Email} already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            UserType = dto.UserType, 
            IsActive = true,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PasswordHash = HashPassword(dto.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }
    private string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new NotFoundException($"User with ID {id} not found.");

        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
        if (dto.EmailVerified.HasValue) user.EmailVerified = dto.EmailVerified.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new NotFoundException($"User with ID {id} not found.");

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserType = user.UserType,
            IsActive = user.IsActive,
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt
        };
    }
}
