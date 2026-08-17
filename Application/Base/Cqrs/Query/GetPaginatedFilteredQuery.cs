using System.Linq.Expressions;
using Domain.DomainShared;
using MediatR;

namespace Application.Base;

/// <summary>
/// Paginado con filtro — transversal a cualquier módulo (ENT, DTO). El predicado se
/// construye en el controlador concreto, en términos de la entidad de dominio, y viaja
/// tal cual hasta el repositorio (traducido a SQL por EF Core).
/// </summary>
public record GetPaginatedFilteredQuery<ENT, DTO>(int PageNumber, int PageSize, Expression<Func<ENT, bool>> Predicate) : IRequest<PaginatedList<DTO>>
    where ENT : class, new()
    where DTO : class, new();

public class GetPaginatedFilteredHandler<ENT, DTO> : IRequestHandler<GetPaginatedFilteredQuery<ENT, DTO>, PaginatedList<DTO>>
    where ENT : class, new()
    where DTO : class, new()
{
    private readonly IApplicationReadOnlyService<ENT, DTO> _service;

    public GetPaginatedFilteredHandler(IApplicationReadOnlyService<ENT, DTO> service)
    {
        _service = service;
    }

    public async Task<PaginatedList<DTO>> Handle(GetPaginatedFilteredQuery<ENT, DTO> request, CancellationToken cancellationToken)
    {
        return await _service.GetPaginatedAsync(request.PageNumber, request.PageSize, request.Predicate);
    }
}
