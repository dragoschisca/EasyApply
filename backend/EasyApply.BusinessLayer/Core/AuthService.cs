using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using EasyApply.BusinessLayer.Structure.DTOs.Auth;
using EasyApply.BusinessLayer.Interfaces.Services;
using EasyApply.Domain.Entities;
using EasyApply.Domain.Enums;
using EasyApply.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EasyApply.BusinessLayer.Core;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        ICandidateRepository candidateRepository,
        ICompanyRepository companyRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _candidateRepository = candidateRepository;
        _companyRepository = companyRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new Exception("User with this email already exists.");
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
            throw new Exception("Invalid email or password.");
        }

        var token = GenerateJwtToken(user);

        string firstName = string.Empty;
        string lastName = string.Empty;

        if (user.UserType == UserType.Candidate)
        {
            var candidates = await _candidateRepository.GetAllAsync();
            var candidate = candidates.FirstOrDefault(c => c.UserId == user.Id);
            if (candidate != null)
            {
                firstName = candidate.FirstName;
                lastName = candidate.LastName;
            }
        }
        else if (user.UserType == UserType.Company)
        {
            var companies = await _companyRepository.GetAllAsync();
            var company = companies.FirstOrDefault(c => c.UserId == user.Id);
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
