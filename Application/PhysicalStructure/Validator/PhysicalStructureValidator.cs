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
    }
}
