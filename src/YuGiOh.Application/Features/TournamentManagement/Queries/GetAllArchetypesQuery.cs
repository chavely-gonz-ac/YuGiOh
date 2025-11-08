using Ardalis.Specification;
using MediatR;
using YuGiOh.Application.Features.Common.DTOs;
using YuGiOh.Domain.DTOs;
using YuGiOh.Domain.Models;

namespace YuGiOh.Application.Features.TournamentManagement.Queries
{
    public record GetAllArchetypesQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<Archetype>>;

    public class GetAllArchetypesQueryHandler : IRequestHandler<GetAllArchetypesQuery, PagedResult<Archetype>>
    {
        private readonly IReadRepositoryBase<Archetype> _archetypeRepository;

        public GetAllArchetypesQueryHandler(IReadRepositoryBase<Archetype> archetypeRepository)
        {
            _archetypeRepository = archetypeRepository;
        }

        public async Task<PagedResult<Archetype>> Handle(GetAllArchetypesQuery request, CancellationToken cancellationToken)
        {
            var pagination = new Pagination(request.Page, request.PageSize);

            var spec = new PagedArchetypesSpecification(pagination);
            var items = await _archetypeRepository.ListAsync(spec, cancellationToken);
            var totalCount = await _archetypeRepository.CountAsync(cancellationToken);

            return new PagedResult<Archetype>(items, totalCount, pagination.Page, pagination.PageSize);
        }
    }


    /// <summary>
    /// Specification for retrieving paged Decks with their related Archetypes and Owners.
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
