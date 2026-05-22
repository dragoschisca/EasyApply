using System.Text.RegularExpressions;
using EasyApply.BusinessLayer.Structure.DTOs.Auth;
using EasyApply.BusinessLayer.Structure.DTOs.Company;
using EasyApply.BusinessLayer.Structure.DTOs.Job;
using EasyApply.BusinessLayer.Structure.DTOs.Application;
using EasyApply.Domain.Enums;
using EasyApply.Domain.Exceptions;

namespace EasyApply.BusinessLayer.Structure.Validation;

public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static void ValidateRegister(RegisterRequestDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            AddError(errors, nameof(dto.Email), "Email is required.");
        }
        else if (!EmailRegex.IsMatch(dto.Email))
        {
            AddError(errors, nameof(dto.Email), "Email is invalid.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            AddError(errors, nameof(dto.Password), "Password is required.");
        }
        else if (dto.Password.Length < 6)
        {
            AddError(errors, nameof(dto.Password), "Password must be at least 6 characters long.");
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            AddError(errors, nameof(dto.ConfirmPassword), "Passwords do not match.");
        }

        if (dto.UserType == UserType.Candidate)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                AddError(errors, nameof(dto.FirstName), "First Name is required for candidate registration.");
            }
            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                AddError(errors, nameof(dto.LastName), "Last Name is required for candidate registration.");
            }
        }
        else if (dto.UserType == UserType.Company)
        {
            if (string.IsNullOrWhiteSpace(dto.CompanyName))
            {
                AddError(errors, nameof(dto.CompanyName), "Company Name is required for company registration.");
            }
        }
        else
        {
            AddError(errors, nameof(dto.UserType), "Invalid user type specified.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateLogin(LoginRequestDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            AddError(errors, nameof(dto.Email), "Email is required.");
        }
        else if (!EmailRegex.IsMatch(dto.Email))
        {
            AddError(errors, nameof(dto.Email), "Email is invalid.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            AddError(errors, nameof(dto.Password), "Password is required.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateCreateCompany(CreateCompanyDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (dto.UserId == Guid.Empty)
        {
            AddError(errors, nameof(dto.UserId), "User ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.CompanyName))
        {
            AddError(errors, nameof(dto.CompanyName), "Company Name is required.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateUpdateCompany(UpdateCompanyDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (dto.CompanyName != null && string.IsNullOrWhiteSpace(dto.CompanyName))
        {
            AddError(errors, nameof(dto.CompanyName), "Company Name cannot be empty.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateCreateJob(CreateJobDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (dto.CompanyId == Guid.Empty)
        {
            AddError(errors, nameof(dto.CompanyId), "Company ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            AddError(errors, nameof(dto.Title), "Job Title is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Category))
        {
            AddError(errors, nameof(dto.Category), "Job Category is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            AddError(errors, nameof(dto.Description), "Job Description is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Requirements))
        {
            AddError(errors, nameof(dto.Requirements), "Job Requirements are required.");
        }

        if (dto.SalaryMin.HasValue && dto.SalaryMin.Value < 0)
        {
            AddError(errors, nameof(dto.SalaryMin), "Minimum salary cannot be negative.");
        }

        if (dto.SalaryMax.HasValue && dto.SalaryMax.Value < 0)
        {
            AddError(errors, nameof(dto.SalaryMax), "Maximum salary cannot be negative.");
        }

        if (dto.SalaryMin.HasValue && dto.SalaryMax.HasValue && dto.SalaryMax.Value < dto.SalaryMin.Value)
        {
            AddError(errors, nameof(dto.SalaryMax), "Maximum salary cannot be less than minimum salary.");
        }

        if (dto.ExpiresAt.HasValue && dto.ExpiresAt.Value <= DateTime.UtcNow)
        {
            AddError(errors, nameof(dto.ExpiresAt), "Expiration date must be in the future.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateUpdateJob(UpdateJobDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (dto.Title != null && string.IsNullOrWhiteSpace(dto.Title))
        {
            AddError(errors, nameof(dto.Title), "Job Title cannot be empty.");
        }

        if (dto.Category != null && string.IsNullOrWhiteSpace(dto.Category))
        {
            AddError(errors, nameof(dto.Category), "Job Category cannot be empty.");
        }

        if (dto.Description != null && string.IsNullOrWhiteSpace(dto.Description))
        {
            AddError(errors, nameof(dto.Description), "Job Description cannot be empty.");
        }

        if (dto.Requirements != null && string.IsNullOrWhiteSpace(dto.Requirements))
        {
            AddError(errors, nameof(dto.Requirements), "Job Requirements cannot be empty.");
        }

        if (dto.SalaryMin.HasValue && dto.SalaryMin.Value < 0)
        {
            AddError(errors, nameof(dto.SalaryMin), "Minimum salary cannot be negative.");
        }

        if (dto.SalaryMax.HasValue && dto.SalaryMax.Value < 0)
        {
            AddError(errors, nameof(dto.SalaryMax), "Maximum salary cannot be negative.");
        }

        if (dto.SalaryMin.HasValue && dto.SalaryMax.HasValue && dto.SalaryMax.Value < dto.SalaryMin.Value)
        {
            AddError(errors, nameof(dto.SalaryMax), "Maximum salary cannot be less than minimum salary.");
        }

        if (dto.ExpiresAt.HasValue && dto.ExpiresAt.Value <= DateTime.UtcNow)
        {
            AddError(errors, nameof(dto.ExpiresAt), "Expiration date must be in the future.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateCreateApplication(CreateApplicationDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (dto.JobId == Guid.Empty)
        {
            AddError(errors, nameof(dto.JobId), "Job ID is required.");
        }

        if (dto.CVId == Guid.Empty)
        {
            AddError(errors, nameof(dto.CVId), "CV ID is required.");
        }

        ThrowIfHasErrors(errors);
    }

    public static void ValidateUpdateApplicationStatus(UpdateApplicationStatusDto dto)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(dto.Status))
        {
            AddError(errors, nameof(dto.Status), "Status is required.");
        }
        else if (dto.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(dto.Feedback))
            {
                AddError(errors, nameof(dto.Feedback), "Rejection feedback is required when status is 'Rejected'.");
            }
        }

        ThrowIfHasErrors(errors);
    }

    private static void AddError(Dictionary<string, string[]> errors, string key, string errorMessage)
    {
        if (errors.TryGetValue(key, out var existingErrors))
        {
            var newErrors = new List<string>(existingErrors) { errorMessage };
            errors[key] = newErrors.ToArray();
        }
        else
        {
            errors[key] = new[] { errorMessage };
        }
    }

    private static void ThrowIfHasErrors(Dictionary<string, string[]> errors)
    {
        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}
