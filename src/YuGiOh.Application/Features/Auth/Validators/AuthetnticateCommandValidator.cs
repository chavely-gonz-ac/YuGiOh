using FluentValidation;
using YuGiOh.Application.Features.Auth.Commands;

namespace YuGiOh.Application.Features.Auth.Validators
{
    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {
        public AuthenticateCommandValidator()
        {
            RuleFor(x => x.Handler)
                .NotEmpty().WithMessage("Username or email is required.")
                .MaximumLength(255)
                // optional — detect obvious invalid emails if they are provided
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .When(x => x.Handler.Contains('@'))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("IpAddress is required.")
                .Matches(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$|^([a-fA-F0-9:]+)$")
                .WithMessage("Invalid IP address format.");
        }
    }
}
