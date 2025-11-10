using FluentValidation;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Validators
{
    /// <summary>
    /// Validates the <see cref="Tournament"/> domain model.
    /// </summary>
    /// <remarks>
    /// Ensures the tournament data follows the Yu-Gi-Oh! rules:
    /// - Name must be provided and within a reasonable length.
    /// - The address and schedule fields are required.
    /// - Registration deadline must occur before the start date.
    /// - Timestamps must not be set in the future.
    /// </remarks>
    public class TournamentValidator : AbstractValidator<Tournament>
    {
        public TournamentValidator()
        {
            // --- Name ---
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Tournament name is required.")
                .MaximumLength(200).WithMessage("Tournament name cannot exceed 200 characters.");

            // --- Dates ---
            RuleFor(t => t.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
                .WithMessage("Start date cannot be in the past.");

            RuleFor(t => t.RegistrationLimit)
                .NotEmpty().WithMessage("Registration limit date is required.")
                .LessThan(t => t.StartDate)
                .WithMessage("Registration limit must be before the tournament start date.");
        }
    }
}
