using FluentValidation;
using YuGiOh.Application.Features.TournamentManagement.Commands;

namespace YuGiOh.Application.Features.TournamentManagement.Validators
{
    /// <summary>
    /// Validates the <see cref="RegisterTournamentCommand"/>.
    /// </summary>
    public class RegisterTournamentCommandValidator : AbstractValidator<RegisterTournamentCommand>
    {
        public RegisterTournamentCommandValidator()
        {
            RuleFor(c => c.Tournament)
                .NotNull().WithMessage("Tournament cannot be null.")
                .SetValidator(new TournamentValidator());

            RuleFor(x => x.SponsoredBy)
                .NotNull().WithMessage("At least one sponsor must be provided.")
                .Must(r => r.Any()).WithMessage("At least one sponsor must be provided.")
                .ForEach(r => r.NotEmpty().WithMessage("Sponsor names cannot be empty."));
        }
    }
}
