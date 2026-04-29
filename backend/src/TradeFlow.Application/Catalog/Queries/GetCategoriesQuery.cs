using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Application.Catalog.Queries;

public record CategoryDto(Guid Id, string Name, string Slug);

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public class GetCategoriesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        => await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id.Value, c.Name, c.Slug))
            .ToListAsync(ct);
}
