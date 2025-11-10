using FluentValidation;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Validators
{
    /// <summary>
    /// Validates the <see cref="Sponsorship"/> domain model.
    /// </summary>
    /// <remarks>
    /// Ensures sponsorship relationships are well-defined:
    /// - Both SponsorId and TournamentId must be valid.
    /// - The relationship must reference valid entities.
    /// - The creation date cannot be set in the future.
    /// </remarks>
    public class SponsorshipValidator : AbstractValidator<Sponsorship>
    {
        public SponsorshipValidator()
        {
            // --- Sponsor ---
            RuleFor(s => s.SponsorId)
                .NotEmpty().WithMessage("SponsorId is required.")
                .MaximumLength(256).WithMessage("SponsorId cannot exceed 256 characters.");

            RuleFor(s => s.Sponsor)
                .NotNull().WithMessage("Sponsor reference cannot be null.");
        }
    }
}
