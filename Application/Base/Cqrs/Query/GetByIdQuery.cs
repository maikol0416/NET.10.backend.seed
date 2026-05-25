using MediatR;

namespace Application.Base;

public record GetByIdQuery<ENT, DTO>(int Id) : IRequest<DTO?>
    where ENT : class, new()
    where DTO : class, new();

public class GetByIdHandler<ENT, DTO> : IRequestHandler<GetByIdQuery<ENT, DTO>, DTO?>
    where ENT : class, new()
    where DTO : class, new()
{
    private readonly IApplicationReadOnlyService<ENT, DTO> _service;

    public GetByIdHandler(IApplicationReadOnlyService<ENT, DTO> service)
    {
        _service = service;
    }

    public async Task<DTO?> Handle(GetByIdQuery<ENT, DTO> request, CancellationToken cancellationToken)
    {
        return await _service.GetByIdAsync(request.Id);
    }
}

