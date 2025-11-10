using Ardalis.Specification;
using MediatR;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Commands
{
    public record RegisterTournamentCommand : IRequest<Tournament>
    {
        public required Tournament Tournament { get; set; }
        public required IEnumerable<string> SponsoredBy { get; set; }
    }

    public class RegisterTournamentCommandHandler : IRequestHandler<RegisterTournamentCommand, Tournament>
    {
        private readonly IRepositoryBase<Tournament> _tournamentRepository;
        private readonly IRepositoryBase<Address> _addressRepository;
        private readonly IRepositoryBase<Sponsorship> _sponsorshipRepository;

        public RegisterTournamentCommandHandler(
            IRepositoryBase<Tournament> tournamentRepository,
            IRepositoryBase<Address> addressRepository,
            IRepositoryBase<Sponsorship> sponsorshipRepository
        )
        {
            _tournamentRepository = tournamentRepository;
            _addressRepository = addressRepository;
            _sponsorshipRepository = sponsorshipRepository;
        }
        public async Task<Tournament> Handle(RegisterTournamentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var address = await _addressRepository.SingleOrDefaultAsync
                (new FindAddressSpecification(request.Tournament.Address), cancellationToken);
                address ??= await _addressRepository.AddAsync(request.Tournament.Address, cancellationToken);
                // await _addressRepository.SaveChangesAsync(cancellationToken);
                // address = await _addressRepository.SingleOrDefaultAsync
                // (new FindAddressSpecification(request.Tournament.Address), cancellationToken);
                // Console.WriteLine(address);
                request.Tournament.AddressId = address.Id;
                request.Tournament.Address = null;
                var tournament = await _tournamentRepository.AddAsync(request.Tournament, cancellationToken);
                foreach (string sponsor in request.SponsoredBy)
                {
                    await _sponsorshipRepository.AddAsync(new Sponsorship()
                    {
                        SponsorId = sponsor,
                        TournamentId = tournament.Id
                    }, cancellationToken);
                }
                tournament.Address = null;
                return tournament;
            }
            catch (Exception ex)
            {
                throw APIException.BadRequest("Error while creating the Tournament", innerException: ex);
            }
        }
    }
    public sealed class FindAddressSpecification
    : Specification<Address>, ISingleResultSpecification<Address>
    {
        public FindAddressSpecification(Address address)
        {
            Query
                .Where(
                    a => a.CountryIso2 == address.CountryIso2 &
                    a.StateIso2 == address.StateIso2 &
                    a.City == address.City &
                    a.StreetTypeId == address.StreetTypeId &
                    a.StreetName == address.StreetName &
                    a.Building == address.Building &
                    a.Apartment == address.Apartment
                    // a => a.ToString() == address.ToString()
                );
        }
    }
}
