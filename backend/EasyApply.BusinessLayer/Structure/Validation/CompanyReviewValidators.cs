using System;
using System.Text.RegularExpressions;
using FluentValidation;
using EasyApply.BusinessLayer.Structure.DTOs.CompanyReview;

namespace EasyApply.BusinessLayer.Structure.Validators;

public class CreateCompanyReviewDtoValidator : AbstractValidator<CreateCompanyReviewDto>
{
    private static readonly Regex EmailRegex = new(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"\b\d{7,15}\b|(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}", RegexOptions.Compiled);

    public CreateCompanyReviewDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Review title is required.")
            .Length(10, 200).WithMessage("Title must be between 10 and 200 characters.")
            .Must(text => !ContainsPii(text)).WithMessage("Title must not contain email addresses or phone numbers.");

        RuleFor(x => x.ReviewText)
            .NotEmpty().WithMessage("Review content text is required.")
            .Length(50, 5000).WithMessage("Review content must be between 50 and 5000 characters.")
            .Must(text => !ContainsPii(text)).WithMessage("Review content must not contain email addresses or phone numbers.");

        RuleFor(x => x.JobTitle)
            .MaximumLength(150).WithMessage("Job title cannot exceed 150 characters.")
            .When(x => !string.IsNullOrEmpty(x.JobTitle));

        RuleFor(x => x.SalaryOffered)
            .GreaterThanOrEqualTo(0).WithMessage("Salary offered cannot be negative.")
            .When(x => x.SalaryOffered.HasValue);

        RuleFor(x => x.HiringProcessDuration)
            .InclusiveBetween(1, 365).WithMessage("Hiring process duration must be between 1 and 365 days.")
            .When(x => x.HiringProcessDuration.HasValue);
    }

    private static bool ContainsPii(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return EmailRegex.IsMatch(text) || PhoneRegex.IsMatch(text);
    }
}

public class UpdateCompanyReviewDtoValidator : AbstractValidator<UpdateCompanyReviewDto>
{
    private static readonly Regex EmailRegex = new(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"\b\d{7,15}\b|(\+?\d{1,3}[-.\s]?)?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}", RegexOptions.Compiled);

    public UpdateCompanyReviewDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Review title is required.")
            .Length(10, 200).WithMessage("Title must be between 10 and 200 characters.")
            .Must(text => !ContainsPii(text)).WithMessage("Title must not contain email addresses or phone numbers.");

        RuleFor(x => x.ReviewText)
            .NotEmpty().WithMessage("Review content text is required.")
            .Length(50, 5000).WithMessage("Review content must be between 50 and 5000 characters.")
            .Must(text => !ContainsPii(text)).WithMessage("Review content must not contain email addresses or phone numbers.");

        RuleFor(x => x.JobTitle)
            .MaximumLength(150).WithMessage("Job title cannot exceed 150 characters.")
            .When(x => !string.IsNullOrEmpty(x.JobTitle));

        RuleFor(x => x.SalaryOffered)
            .GreaterThanOrEqualTo(0).WithMessage("Salary offered cannot be negative.")
            .When(x => x.SalaryOffered.HasValue);

        RuleFor(x => x.HiringProcessDuration)
            .InclusiveBetween(1, 365).WithMessage("Hiring process duration must be between 1 and 365 days.")
            .When(x => x.HiringProcessDuration.HasValue);
    }

    private static bool ContainsPii(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;
        return EmailRegex.IsMatch(text) || PhoneRegex.IsMatch(text);
    }
}
