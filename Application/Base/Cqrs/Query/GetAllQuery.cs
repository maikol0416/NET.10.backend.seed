using MediatR;

namespace Application.Base;

public record GetAllQuery<ENT, DTO>() : IRequest<IEnumerable<DTO>>
    where ENT : class, new()
    where DTO : class, new();

public class GetAllHandler<ENT, DTO> : IRequestHandler<GetAllQuery<ENT, DTO>, IEnumerable<DTO>>
    where ENT : class, new()
    where DTO : class, new()
{
    private readonly IApplicationReadOnlyService<ENT, DTO> _service;

    public GetAllHandler(IApplicationReadOnlyService<ENT, DTO> service)
    {
        _service = service;
    }

    public async Task<IEnumerable<DTO>> Handle(GetAllQuery<ENT, DTO> request, CancellationToken cancellationToken)
    {
        return await _service.GetAllAsync();
    }
}
