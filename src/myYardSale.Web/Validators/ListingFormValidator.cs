using FluentValidation;
using myYardSale.Web.Models;

namespace myYardSale.Web.Validators;

public sealed class ListingFormValidator : AbstractValidator<ListingFormViewModel>
{
    public ListingFormValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(300).WithMessage("Title cannot exceed 300 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.")
            .PrecisionScale(10, 2, false).WithMessage("Price has too many decimal places.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid listing status.");

        RuleForEach(x => x.ImageFiles)
            .Must(file => file is null || file.Length <= 5 * 1024 * 1024)
            .WithMessage("Each image must be 5 MB or less.")
            .Must(file => file is null || IsValidImageExtension(file.FileName))
            .WithMessage("Only .jpg, .jpeg, .png, .gif, and .webp images are allowed.");
    }



    private static bool IsValidImageExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp";
    }
}