using FluentValidation;
using EasyApply.BusinessLayer.Structure.DTOs.Candidate;
using System;

namespace EasyApply.BusinessLayer.Structure.Validators;

public class CreateCandidateDtoValidator : AbstractValidator<CreateCandidateDto>
{
    public CreateCandidateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.LinkedInUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("LinkedIn URL must be a valid URL.");

        RuleFor(x => x.GitHubUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.GitHubUrl))
            .WithMessage("GitHub URL must be a valid URL.");
            
        RuleFor(x => x.PortfolioUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.PortfolioUrl))
            .WithMessage("Portfolio URL must be a valid URL.");
    }
}

public class UpdateCandidateDtoValidator : AbstractValidator<UpdateCandidateDto>
{
    public UpdateCandidateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.LinkedInUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("LinkedIn URL must be a valid URL.");

        RuleFor(x => x.GitHubUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.GitHubUrl))
            .WithMessage("GitHub URL must be a valid URL.");
            
        RuleFor(x => x.PortfolioUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.PortfolioUrl))
            .WithMessage("Portfolio URL must be a valid URL.");
    }
}
