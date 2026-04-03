using Application.Base;
using MediatR;

namespace Application.Base;

public class DeleteCommand
{
}


 public record DeleteCommand<ENT, DTO>(int id) : IRequest<bool>
        where ENT : class, new()
        where DTO : class, new();

    public class DeleteHandler<ENT, DTO> : IRequestHandler<DeleteCommand<ENT, DTO>, bool>
        where ENT : class, new()
        where DTO : class, new()
    {
        protected readonly IApplicationService<ENT, DTO> _implementation;

        public DeleteHandler(IApplicationService<ENT, DTO> implementation)
        {
            _implementation = implementation;
        }

        public async Task<bool> Handle(DeleteCommand<ENT, DTO> request, CancellationToken cancellationToken)
        {
            return await _implementation.DeleteEntity(request.id);
        }
    }