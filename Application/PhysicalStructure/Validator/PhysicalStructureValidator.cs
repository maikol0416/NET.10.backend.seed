using Application.Dto;
using Domain.Ports;
using FluentValidation;

namespace Application.Validator;

public class PhysicalStructureValidator:AbstractValidator<PhysicalStructureDto>
{   
    private readonly IPhysicalStructureRepository _physicalStructureRepository;
    public PhysicalStructureValidator(IPhysicalStructureRepository physicalStructureRepository)
    {
        _physicalStructureRepository = physicalStructureRepository;

        // TODO: es solo un ejemplo de implenetación de reglas

        RuleFor(x => x.Name).NotEmpty().WithErrorCode($"NameEmpty")
        .WithMessage(x => x.Name)
                .WithName(nameof(PhysicalStructureDto.Name));

        // AdministratorUserId es opcional (puede no haber administrador asignado aún),
        // pero si viene debe respetar el ancho de columna del Id de AspNetUsers (nvarchar(450)).
        RuleFor(x => x.AdministratorUserId)
            .MaximumLength(450)
            .WithErrorCode("AdministratorUserIdTooLong")
            .WithMessage("El identificador del administrador no puede exceder los 450 caracteres.")
            .WithName(nameof(PhysicalStructureDto.AdministratorUserId))
            .When(x => !string.IsNullOrEmpty(x.AdministratorUserId));
    }
}
