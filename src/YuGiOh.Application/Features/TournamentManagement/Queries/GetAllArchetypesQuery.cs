using Ardalis.Specification;
using MediatR;
using YuGiOh.Application.Features.Common.DTOs;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;
using YuGiOh.Domain.Repositories;

namespace YuGiOh.Application.Features.TournamentManagement.Queries
{
    /// <summary>
    /// Query to retrieve a paginated list of Archetypes.
    /// Results are cached for faster access and reduced database load.
    /// </summary>
    public record GetAllArchetypesQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<Archetype>>;

    /// <summary>
    /// Handles the <see cref="GetAllArchetypesQuery"/> request using caching.
    /// </summary>
    public class GetAllArchetypesQueryHandler : IRequestHandler<GetAllArchetypesQuery, PagedResult<Archetype>>
    {
        private readonly IReadRepositoryBase<Archetype> _archetypeRepository;
        private readonly ICachingRepository _cache;

        public GetAllArchetypesQueryHandler(
            IReadRepositoryBase<Archetype> archetypeRepository,
            ICachingRepository cache)
        {
            _archetypeRepository = archetypeRepository;
            _cache = cache;
        }

        public async Task<PagedResult<Archetype>> Handle(GetAllArchetypesQuery request, CancellationToken cancellationToken)
        {
            var pagination = new Pagination(request.Page, request.PageSize);
            var cacheKey = $"archetypes_page_{pagination.Page}_size_{pagination.PageSize}";

            // Retrieve from cache or create new entry
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var spec = new PagedArchetypesSpecification(pagination);
                    var items = await _archetypeRepository.ListAsync(spec, cancellationToken);
                    var totalCount = await _archetypeRepository.CountAsync(cancellationToken);

                    return new PagedResult<Archetype>(
                        items,
                        totalCount,
                        pagination.Page,
                        pagination.PageSize
                    );
                },
                expiry: TimeSpan.FromMinutes(10) // Cache duration (tweak as needed)
            );
        }
    }

    /// <summary>
    /// Specification for retrieving paginated Archetypes.
    /// </summary>
    public sealed class PagedArchetypesSpecification : Specification<Archetype>
    {
        public PagedArchetypesSpecification(Pagination pagination)
        {
            Query
                .Skip(pagination.Skip)
                .Take(pagination.Take);
        }
    }
}
