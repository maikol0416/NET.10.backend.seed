using Domain.DomainShared;
using MediatR;

namespace Application.Base;

public record GetPaginatedQuery<ENT, DTO>(int PageNumber, int PageSize) : IRequest<PaginatedList<DTO>>
    where ENT : class, new()
    where DTO : class, new();

public class GetPaginatedHandler<ENT, DTO> : IRequestHandler<GetPaginatedQuery<ENT, DTO>, PaginatedList<DTO>>
    where ENT : class, new()
    where DTO : class, new()
{
    private readonly IApplicationReadOnlyService<ENT, DTO> _service;

    public GetPaginatedHandler(IApplicationReadOnlyService<ENT, DTO> service)
    {
        _service = service;
    }

    public async Task<PaginatedList<DTO>> Handle(GetPaginatedQuery<ENT, DTO> request, CancellationToken cancellationToken)
    {
        return await _service.GetPaginatedAsync(request.PageNumber, request.PageSize);
    }
}
