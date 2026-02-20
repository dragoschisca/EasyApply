namespace EasyApplyAPI.DTOs.Auth;

public class RegisterRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    //public UserType UserType { get; set; }

    // Candidate fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Company fields
    public string? CompanyName { get; set; }
}