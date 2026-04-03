using MediatR;

namespace Application.Base;

public record UpdateCommand<ENT, DTO>(DTO Dto) : IRequest<DTO>
        where ENT : class, new()
        where DTO : class, new();

    public class UpdateHandler<ENT, DTO> : IRequestHandler<UpdateCommand<ENT, DTO>, DTO>
        where ENT : class, new()
        where DTO : class, new()
    {
        protected readonly IApplicationService<ENT, DTO> _implementation;

        public UpdateHandler(IApplicationService<ENT, DTO> implementation)
        {
            _implementation = implementation;
        }

        public async Task<DTO> Handle(UpdateCommand<ENT, DTO> request, CancellationToken cancellationToken)
        {
            return await _implementation.UpdateAsync(request.Dto);
        }
    }
