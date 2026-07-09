using Application.Dto;
using Domain.BoundedContext.Tenancy;
using Domain.Ports;
using MediatR;

namespace Application.ManagementCompany.Commands;

public record CreateManagementCompanyCommand(ManagementCompanyDto ManagementCompanyDto) : IRequest<Guid>;

public class CreateManagementCompanyCommandHandler
    : IRequestHandler<CreateManagementCompanyCommand, Guid>
{
    private readonly IManagementCompanyRepository _repository;

    public CreateManagementCompanyCommandHandler(IManagementCompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateManagementCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var managementCompany = new ManagementCompanyAgg(
            request.ManagementCompanyDto.Name,
            request.ManagementCompanyDto.Nit,
            request.ManagementCompanyDto.ContactEmail,
            request.ManagementCompanyDto.ContactPhone
        );

        await _repository.CreateAsync(managementCompany);
        return managementCompany.Id;
    }
}
