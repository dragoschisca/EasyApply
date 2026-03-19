namespace EasyApply.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public UserAuthDto User { get; set; } = null!;
}

