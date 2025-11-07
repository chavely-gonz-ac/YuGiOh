using FluentValidation;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.Common.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(a => a.CountryIso2)
                .NotEmpty().WithMessage("Country code is required.")
                .Length(2).WithMessage("Country code must be 2 characters.");

            RuleFor(a => a.StateIso2)
                .MaximumLength(10)
                .When(a => !string.IsNullOrEmpty(a.StateIso2));

            RuleFor(a => a.City)
                .MaximumLength(100)
                .When(a => !string.IsNullOrEmpty(a.City));

            RuleFor(a => a.StreetName)
                .MaximumLength(200)
                .When(a => !string.IsNullOrEmpty(a.StreetName));

            RuleFor(a => a.Building)
                .MaximumLength(100)
                .When(a => !string.IsNullOrEmpty(a.Building));

            RuleFor(a => a.Apartment)
                .MaximumLength(50)
                .When(a => !string.IsNullOrEmpty(a.Apartment));
        }
    }
}
