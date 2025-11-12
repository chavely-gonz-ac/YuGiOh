using FluentValidation;
using YuGiOh.Application.Features.TournamentManagement.Commands;

namespace YuGiOh.Application.Features.TournamentManagement.Validators
{
    /// <summary>
    /// Validator for the <see cref="RegisterUserInTournamentCommand"/>.
    /// </summary>
    /// <remarks>
    /// Ensures that both the <c>TournamentId</c> and <c>DeckId</c> values
    /// are valid before processing the registration.
    /// </remarks>
    public class RegisterUserInTournamentCommandValidator
        : AbstractValidator<RegisterUserInTournamentCommand>
    {
        public RegisterUserInTournamentCommandValidator()
        {
            // --- Tournament ID Validation ---
            RuleFor(c => c.TournamentsId);

            // --- Deck ID Validation ---
            RuleFor(c => c.DecksId);
        }
    }
}
