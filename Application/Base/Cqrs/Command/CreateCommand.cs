using MediatR;

namespace Application.Base;

public record CreateCommand<ENT, DTO>(DTO Dto) : IRequest<DTO>
		where ENT : class, new()
		where DTO : class, new();

public class CreateHandler<ENT, DTO> : IRequestHandler<CreateCommand<ENT, DTO>, DTO>
	where ENT : class, new()
	where DTO : class, new()
{
	protected readonly IApplicationService<ENT, DTO> _implementation;

	public CreateHandler(IApplicationService<ENT, DTO> implementation)
	{
		_implementation = implementation;
	}

	public async Task<DTO> Handle(CreateCommand<ENT, DTO> request, CancellationToken cancellationToken)
	{
		return  await _implementation.CreateAsync(request.Dto);
	}
}