using Domain.BoundedContext.Properties;
using Domain.Ports;
using MediatR;

namespace Application.PhysicalStructure.Commands;

public record CreatePhysicalStructureCommand(PhysicalStructureDto PhysicalStructureDto) : IRequest<Guid>;

public class CreatePhysicalStructureCommandHandler 
    : IRequestHandler<CreatePhysicalStructureCommand, Guid>
{
    private readonly IPhysicalStructureRepository _repository;

    public CreatePhysicalStructureCommandHandler(IPhysicalStructureRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreatePhysicalStructureCommand request, 
        CancellationToken cancellationToken)
    {
        var location = new LocationValueObject (request.PhysicalStructureDto.Street, request.PhysicalStructureDto.Number);
        var structure = new PhysicalStructureAgg(request.PhysicalStructureDto.Name, location);
        await _repository.CreateAsync(structure);
        return structure.Id;
    }
}
