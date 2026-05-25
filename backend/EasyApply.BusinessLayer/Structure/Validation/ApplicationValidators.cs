using FluentValidation;
using EasyApply.BusinessLayer.Structure.DTOs.Application;
using System;

namespace EasyApply.BusinessLayer.Structure.Validators;

public class CreateApplicationDtoValidator : AbstractValidator<CreateApplicationDto>
{
    public CreateApplicationDtoValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Job ID is required.")
            .NotEqual(Guid.Empty).WithMessage("A valid Job ID is required.");

        RuleFor(x => x.CVId)
            .NotEmpty().WithMessage("CV ID is required.")
            .NotEqual(Guid.Empty).WithMessage("A valid CV ID is required.");
    }
}

public class UpdateApplicationStatusDtoValidator : AbstractValidator<UpdateApplicationStatusDto>
{
    public UpdateApplicationStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => s == "Pending" || s == "Reviewed" || s == "Accepted" || s == "Rejected")
            .WithMessage("Status must be one of: Pending, Reviewed, Accepted, Rejected.");
    }
}
