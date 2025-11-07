using FluentValidation;

using YuGiOh.Domain.DTOs;
using YuGiOh.Application.Features.Common.Validators;

namespace YuGiOh.Application.Features.Auth.Validators
{
    public class RegisterRequestDataValidator : AbstractValidator<RegisterRequestData>
    {
        public RegisterRequestDataValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(255);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100)
                .Matches("^[A-Za-z]+$").WithMessage("First name must contain only Latin letters (A-Z, a-z).");

            RuleFor(x => x.MiddleName)
                .MaximumLength(100)
                .Matches("^[A-Za-z]+$")
                    .When(x => !string.IsNullOrWhiteSpace(x.MiddleName))
                    .WithMessage("Middle name must contain only Latin letters (A-Z, a-z).");

            RuleFor(x => x.FirstSurname)
                .NotEmpty().WithMessage("First surname is required.")
                .MaximumLength(100)
                .Matches("^[A-Za-z]+$").WithMessage("First surname must contain only Latin letters (A-Z, a-z).");

            RuleFor(x => x.SecondSurname)
                .NotEmpty().WithMessage("Second surname is required.")
                .MaximumLength(100)
                .Matches("^[A-Za-z]+$").WithMessage("Second surname must contain only Latin letters (A-Z, a-z).");


            RuleFor(x => x.Roles)
                .NotNull().WithMessage("At least one role must be provided.")
                .Must(r => r.Any()).WithMessage("At least one role must be provided.")
                .ForEach(r => r.NotEmpty().WithMessage("Role names cannot be empty."));

            // Nested Address validator
            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address!)
                    .SetValidator(new AddressValidator());
            });

            // Conditional IBAN rule
            When(x => x.Roles.Contains("Sponsor"), () =>
            {
                RuleFor(x => x.IBAN)
                    .NotEmpty().WithMessage("IBAN is required for sponsors.")
                    .Length(15, 34).WithMessage("IBAN must be between 15 and 34 characters.")
                    .Matches("^[A-Z]{2}[0-9A-Z]{13,32}$").WithMessage("Invalid IBAN format.");
            });
        }
    }
}
