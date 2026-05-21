using Application.Dto;
using Domain.Ports;
using FluentValidation;

namespace Application.Validator;

public class DocumentValidator : AbstractValidator<DocumentDto>
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentValidator(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("NameEmpty")
            .WithMessage("El nombre es obligatorio.")
            .WithName(nameof(DocumentDto.Name));

        RuleFor(x => x.Name)
            .MaximumLength(150)
            .WithErrorCode("NameTooLong")
            .WithMessage("El nombre no puede exceder 150 caracteres.")
            .WithName(nameof(DocumentDto.Name));

        RuleFor(x => x.Path)
            .NotEmpty()
            .WithErrorCode("PathEmpty")
            .WithMessage("La ruta es obligatoria.")
            .WithName(nameof(DocumentDto.Path));
    }
}
