using FluentValidation;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Validators
{
    /// <summary>
    /// Validates the <see cref="Deck"/> domain model.
    /// </summary>
    /// <remarks>
    /// This validator ensures:
    /// - Name is required, unique-length, and properly formatted.
    /// - Deck sizes (main, side, extra) are within expected Yu-Gi-Oh! limits.
    /// - OwnerId and ArchetypeId are valid references.
    /// </remarks>
    public class DeckValidator : AbstractValidator<Deck>
    {
        public DeckValidator()
        {
            // --- Name Rules ---
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Deck name is required.")
                .MaximumLength(100).WithMessage("Deck name cannot exceed 100 characters.")
                .Matches(@"^[A-Za-z0-9\s\-\!']+$")
                .WithMessage("Deck name contains invalid characters.");

            // --- Main Deck Rules ---
            RuleFor(d => d.MainDeckSize)
                .InclusiveBetween(40, 60)
                .WithMessage("Main Deck must have between 40 and 60 cards.");

            // --- Side Deck Rules ---
            RuleFor(d => d.SideDeckSize)
                .InclusiveBetween(0, 15)
                .WithMessage("Side Deck can have a maximum of 15 cards.");

            // --- Extra Deck Rules ---
            RuleFor(d => d.ExtraDeckSize)
                .InclusiveBetween(0, 15)
                .WithMessage("Extra Deck can have a maximum of 15 cards.");

            // --- Owner Rules ---
            RuleFor(d => d.OwnerId)
                .NotEmpty().WithMessage("Deck must have an owner.")
                .MaximumLength(256).WithMessage("Owner ID cannot exceed 256 characters.");

            // --- Archetype Rules ---
            RuleFor(d => d.ArchetypeId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Archetype ID must be a non-negative integer.");

            // --- CreatedAt Rules ---
            RuleFor(d => d.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1))
                .WithMessage("CreatedAt cannot be set in the future.");
        }
    }
}
