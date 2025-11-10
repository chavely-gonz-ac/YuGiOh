using Ardalis.Specification;
using MediatR;
using YuGiOh.Application.Features.Common.DTOs;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;
using YuGiOh.Domain.Repositories;

namespace YuGiOh.Application.Features.TournamentManagement.Queries
{
    /// <summary>
    /// Query to retrieve a paginated list of tournaments
    /// whose registration period is still open (RegistrationLimit > now).
    /// Results are cached for faster subsequent access.
    /// </summary>
    public record GetAllOpenTournamentsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<Tournament>>;

    /// <summary>
    /// Handles the <see cref="GetAllOpenTournamentsQuery"/> request using a caching layer.
    /// </summary>
    public class GetAllOpenTournamentsQueryHandler : IRequestHandler<GetAllOpenTournamentsQuery, PagedResult<Tournament>>
    {
        private readonly IReadRepositoryBase<Tournament> _tournamentRepository;
        private readonly ICachingRepository _cache;

        public GetAllOpenTournamentsQueryHandler(
            IReadRepositoryBase<Tournament> tournamentRepository,
            ICachingRepository cache)
        {
            _tournamentRepository = tournamentRepository;
            _cache = cache;
        }

        public async Task<PagedResult<Tournament>> Handle(GetAllOpenTournamentsQuery request, CancellationToken cancellationToken)
        {
            var pagination = new Pagination(request.Page, request.PageSize);
            var cacheKey = $"open_tournaments_page_{pagination.Page}_size_{pagination.PageSize}";

            // Retrieve from cache or generate if missing
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var spec = new PagedTournamentsSpecification(pagination);
                    var items = await _tournamentRepository.ListAsync(spec, cancellationToken);
                    var totalCount = await _tournamentRepository.CountAsync(cancellationToken);

                    return new PagedResult<Tournament>(
                        items,
                        totalCount,
                        pagination.Page,
                        pagination.PageSize
                    );
                },
                expiry: TimeSpan.FromMinutes(5) // Cache validity duration
            );
        }
    }

    /// <summary>
    /// Specification for retrieving paginated tournaments whose registration is still open.
    /// </summary>
    public sealed class PagedTournamentsSpecification : Specification<Tournament>
    {
        public PagedTournamentsSpecification(Pagination pagination)
        {
            Query
                .Where(t => t.RegistrationLimit > DateTime.UtcNow)
                .OrderBy(t => t.RegistrationLimit)
                .Skip(pagination.Skip)
                .Take(pagination.Take);
        }
    }
}
