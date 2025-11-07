using FluentValidation;
using YuGiOh.Application.Features.Auth.Commands;

namespace YuGiOh.Application.Features.Auth.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Data)
                .NotNull().WithMessage("Registration data is required.")
                .SetValidator(new RegisterRequestDataValidator());
        }
    }
}
