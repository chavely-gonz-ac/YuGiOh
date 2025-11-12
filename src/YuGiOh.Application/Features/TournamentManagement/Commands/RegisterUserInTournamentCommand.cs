using Ardalis.Specification;
using MediatR;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Commands
{
    public record RegisterUserInTournamentCommand : IRequest
    {
        public required List<int> TournamentsId { get; set; }
        public required List<int> DecksId { get; set; }
    }

    public class RegisterUserInTournamentCommandHandler : IRequestHandler<RegisterUserInTournamentCommand>
    {
        private readonly IRepositoryBase<Registration> _registrationRepository;

        public RegisterUserInTournamentCommandHandler(
            IRepositoryBase<Registration> registrationRepository
        )
        {
            _registrationRepository = registrationRepository;
        }
        public async Task Handle(RegisterUserInTournamentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<Registration> registrations = new List<Registration>();
                foreach (int tId in request.TournamentsId)
                    foreach (int dId in request.DecksId)

                        registrations.Add(new()
                        {
                            DeckId = dId,
                            TournamentId = tId,
                        });
                await _registrationRepository.AddRangeAsync(registrations, cancellationToken);
            }
            catch (APIException) { throw; }
            catch (Exception ex)
            {
                throw APIException.BadRequest("Error while adding the Registration", innerException: ex);
            }
        }
    }
}
