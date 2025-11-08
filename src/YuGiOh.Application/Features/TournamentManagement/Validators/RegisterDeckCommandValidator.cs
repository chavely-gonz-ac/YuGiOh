using FluentValidation;
using YuGiOh.Application.Features.TournamentManagement.Commands;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Validators
{
    /// <summary>
    /// Validator for the <see cref="RegisterDeckCommand"/>.
    /// </summary>
    /// <remarks>
    /// Ensures the command is not null and the contained <see cref="Deck"/> satisfies
    /// domain rules defined in <see cref="DeckValidator"/>.
    /// </remarks>
    public class RegisterDeckCommandValidator : AbstractValidator<RegisterDeckCommand>
    {
        public RegisterDeckCommandValidator()
        {
            // --- Ensure the command's Deck is not null ---
            RuleFor(c => c.Deck)
                .NotNull()
                .WithMessage("Deck cannot be null.")
                .SetValidator(new DeckValidator());
        }
    }
}
