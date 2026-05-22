using EasyApply.Domain.Enums;

namespace EasyApply.BusinessLayer.Structure.DTOs.User;

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.Candidate; 
}