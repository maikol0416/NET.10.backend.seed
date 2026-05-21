using Application.Dto;
using Domain.BoundedContext.DocumentManagement;
using Domain.Ports;
using MediatR;

namespace Application.Document.Commands;

public record CreateDocumentCommand(DocumentDto DocumentDto) : IRequest<Guid>;

public class CreateDocumentCommandHandler
    : IRequestHandler<CreateDocumentCommand, Guid>
{
    private readonly IDocumentRepository _repository;

    public CreateDocumentCommandHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var signatures = request.DocumentDto.Signatures
            .Select(s => new SignatureValueObject(s.Name, s.Rol))
            .ToList();

        var document = new DocumentAgg(
            request.DocumentDto.Name,
            request.DocumentDto.Description,
            request.DocumentDto.Path,
            signatures
        );

        await _repository.CreateAsync(document);
        return document.Id;
    }
}
