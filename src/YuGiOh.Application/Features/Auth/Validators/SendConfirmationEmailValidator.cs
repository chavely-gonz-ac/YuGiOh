using FluentValidation;

using YuGiOh.Application.Features.Auth.Commands;

namespace YuGiOh.Application.Features.Auth.Validators
{
    public class SendConfirmationEmailCommandValidator : AbstractValidator<SendConfirmationEmailCommand>
    {
        public SendConfirmationEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.CallbackURL)
                .NotEmpty().WithMessage("Callback URL is required.")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("Callback URL must be a valid absolute URL.");
        }
    }
}
