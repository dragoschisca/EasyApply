using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EasyApply.BusinessLayer.Structure.DTOs.Auth;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Enums;
using EasyApply.Domain.Interfaces.Repositories;
using EasyApply.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EasyApply.BusinessLayer.Core;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        ICandidateRepository candidateRepository,
        ICompanyRepository companyRepository,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _candidateRepository = candidateRepository;
        _companyRepository = companyRepository;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {


        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            UserType = request.UserType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        if (request.UserType == UserType.Candidate)
        {
            var candidate = new Candidate
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _candidateRepository.AddAsync(candidate);
        }
        else if (request.UserType == UserType.Company)
        {
            var company = new Company
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CompanyName = request.CompanyName ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _companyRepository.AddAsync(company);
        }

        await _userRepository.SaveChangesAsync();

        // Send Welcome Email
        var userName = request.UserType == UserType.Company ? request.CompanyName : request.FirstName;
        _ = _emailService.SendWelcomeEmailAsync(request.Email, userName ?? "User");

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = new UserAuthDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.UserType == UserType.Company ? (request.CompanyName ?? string.Empty) : (request.FirstName ?? string.Empty),
                LastName = user.UserType == UserType.Company ? string.Empty : (request.LastName ?? string.Empty),
                Role = user.UserType.ToString(),
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("This account has been deactivated.");
        }

        var token = GenerateJwtToken(user);

        string firstName = string.Empty;
        string lastName = string.Empty;

        if (user.UserType == UserType.Candidate)
        {
            var candidate = await _candidateRepository.GetByUserIdAsync(user.Id);
            if (candidate != null)
            {
                firstName = candidate.FirstName;
                lastName = candidate.LastName;
            }
        }
        else if (user.UserType == UserType.Company)
        {
            var company = await _companyRepository.GetByUserIdAsync(user.Id);
            if (company != null)
            {
                firstName = company.CompanyName;
            }
        }

        return new AuthResponseDto
        {
            Token = token,
            User = new UserAuthDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = firstName,
                LastName = lastName,
                Role = user.UserType.ToString(),
                CreatedAt = user.CreatedAt
            }
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        var secret = jwtSettings["Secret"] ?? Environment.GetEnvironmentVariable("Jwt__Secret");

        if (string.IsNullOrEmpty(secret))
        {
            throw new BusinessException("JWT Secret is not configured.");
        }

        var issuer = jwtSettings["Issuer"] ?? "EasyApply";
        var audience = jwtSettings["Audience"] ?? "EasyApplyUsers";

        if (!double.TryParse(jwtSettings["ExpiryMinutes"] ?? "60", out double expiryMinutes))
        {
            expiryMinutes = 60;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
