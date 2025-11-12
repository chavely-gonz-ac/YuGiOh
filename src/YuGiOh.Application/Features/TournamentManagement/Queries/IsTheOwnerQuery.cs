using Ardalis.Specification;
using MediatR;
using YuGiOh.Application.Features.Common.DTOs;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Exceptions;
using YuGiOh.Domain.Models;
using YuGiOh.Domain.Repositories;

namespace YuGiOh.Application.Features.TournamentManagement.Queries
{
    /// <summary>
    /// Query to retrieve a paginated list of Archetypes.
    /// Results are cached for faster access and reduced database load.
    /// </summary>
    public record IsTheOwnerQuery(int DeckId, string UserId) : IRequest<bool>;

    /// <summary>
    /// Handles the <see cref="IsTheOwnerQuery"/> request using caching.
    /// </summary>
    public class IsTheOwnerQueryHandler : IRequestHandler<IsTheOwnerQuery, bool>
    {
        private readonly IReadRepositoryBase<Deck> _deckRepository;

        public IsTheOwnerQueryHandler(
            IReadRepositoryBase<Deck> deckRepository,
            ICachingRepository cache)
        {
            _deckRepository = deckRepository;
        }

        public async Task<bool> Handle(IsTheOwnerQuery request, CancellationToken cancellationToken)
        {
            Deck? deck = await _deckRepository.GetByIdAsync(request.DeckId)
                ?? throw APIException.NotFound($"Deck with Id {request.DeckId} not found");
            return deck.OwnerId == request.UserId;
        }
    }

    /// <summary>
    /// Specification for retrieving paginated Archetypes.
    /// </summary>
    public sealed class IsTheOwner : Specification<Archetype>
    {
        public IsTheOwner(Pagination pagination)
        {
            Query
                .Skip(pagination.Skip)
                .Take(pagination.Take);
        }
    }
}
