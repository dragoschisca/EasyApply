using EasyApply.BusinessLayer.Structure.DTOs.Auth;

namespace EasyApply.BusinessLayer.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
}
