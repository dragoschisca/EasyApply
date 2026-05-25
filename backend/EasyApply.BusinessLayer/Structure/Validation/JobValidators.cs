using FluentValidation;
using EasyApply.BusinessLayer.Structure.DTOs.Job;
using EasyApply.Domain.Enums;

namespace EasyApply.BusinessLayer.Structure.Validators;

public class CreateJobDtoValidator : AbstractValidator<CreateJobDto>
{
    public CreateJobDtoValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Requirements)
            .NotEmpty().WithMessage("Requirements are required.");

        RuleFor(x => x.EmploymentType)
            .IsInEnum().WithMessage("Invalid employment type.");

        RuleFor(x => x.ExperienceLevel)
            .IsInEnum().WithMessage("Invalid experience level.");

        RuleFor(x => x.LocationType)
            .IsInEnum().WithMessage("Invalid location type.");

        RuleFor(x => x.Location)
            .NotEmpty().When(x => x.LocationType != LocationType.Remote)
            .WithMessage("Location is required for non-remote jobs.");

        RuleFor(x => x.SalaryMin)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum salary cannot be negative.");

        RuleFor(x => x.SalaryMax)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum salary cannot be negative.")
            .GreaterThanOrEqualTo(x => x.SalaryMin).When(x => x.SalaryMin.HasValue)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.");
    }
}

public class UpdateJobDtoValidator : AbstractValidator<UpdateJobDto>
{
    public UpdateJobDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().When(x => x.Title != null).WithMessage("Job title cannot be empty.")
            .MaximumLength(100).WithMessage("Job title cannot exceed 100 characters.");

        RuleFor(x => x.EmploymentType)
            .IsInEnum().When(x => x.EmploymentType.HasValue).WithMessage("Invalid employment type.");

        RuleFor(x => x.ExperienceLevel)
            .IsInEnum().When(x => x.ExperienceLevel.HasValue).WithMessage("Invalid experience level.");

        RuleFor(x => x.LocationType)
            .IsInEnum().When(x => x.LocationType.HasValue).WithMessage("Invalid location type.");

        RuleFor(x => x.SalaryMin)
            .GreaterThanOrEqualTo(0).When(x => x.SalaryMin.HasValue).WithMessage("Minimum salary cannot be negative.");

        RuleFor(x => x.SalaryMax)
            .GreaterThanOrEqualTo(0).When(x => x.SalaryMax.HasValue).WithMessage("Maximum salary cannot be negative.")
            .GreaterThanOrEqualTo(x => x.SalaryMin).When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.");
    }
}
