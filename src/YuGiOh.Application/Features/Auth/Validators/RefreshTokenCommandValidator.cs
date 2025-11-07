using FluentValidation;
using YuGiOh.Application.Features.Auth.Commands;

namespace YuGiOh.Application.Features.Auth.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MinimumLength(20).WithMessage("Refresh token appears to be invalid (too short).")
                .MaximumLength(500).WithMessage("Refresh token appears to be invalid (too long).")
                // Base64 validation pattern: A-Z, a-z, 0-9, +, /, possibly ending with = padding
                .Matches(@"^[A-Za-z0-9+/]*={0,2}$")
                .WithMessage("Refresh token must be a valid Base64 string.");

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("IP address is required.")
                // IPv4 or IPv6 format
                .Matches(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}$|^([a-fA-F0-9:]+)$")
                .WithMessage("Invalid IP address format.");
        }
    }
}
