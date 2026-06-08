using System.Reflection;
using Application.Auth.Dtos;
using Application.Auth.Validator;
using Application.Base;
using Application.Dto;
using Application.Service;
using Application.Validator;
using Domain.BoundedContext.Properties;
using Domain.BoundedContext.DocumentManagement;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInyection
{
    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services)
    {
        //Validator
        services.RegisterValidators();

        //Cqrs
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(configuration => 
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        services.RegisterMediatrAbstractService<PhysicalStructureService, PhysicalStructureDto, PhysicalStructureAgg, IPhysicalStructureService>();
        services.RegisterMediatrAbstractService<DocumentService, DocumentDto, DocumentAgg, IDocumentService>();

        // Query side — servicios de solo lectura con EntityReadOnlyDbContext (NoTracking)
        services.RegisterMediatrAbstractReadOnlyService<PhysicalStructureReadOnlyService, PhysicalStructureDto, PhysicalStructureAgg, IPhysicalStructureReadOnlyService>();
        services.RegisterMediatrAbstractReadOnlyService<DocumentReadOnlyService, DocumentDto, DocumentAgg, IDocumentReadOnlyService>();
        return services;
    }

    public static void RegisterValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PhysicalStructureDto>, PhysicalStructureValidator>();
        services.AddScoped<IValidator<DocumentDto>, DocumentValidator>();

        // Auth validators
        services.AddScoped<IValidator<AuthLoginDto>, LoginValidator>();
        services.AddScoped<IValidator<AuthRegisterDto>, RegisterValidator>();
        services.AddScoped<IValidator<CreateRoleDto>, CreateRoleValidator>();
    }

    public static void RegisterMediatrAbstractService<Service, DTO, ENT, TImplementation>(this IServiceCollection services)
            where Service : ApplicationService<ENT, DTO>
            where DTO : class, new()
            where ENT : class, new()
            where TImplementation : IApplicationService<ENT, DTO>
        {
            services.AddScoped(typeof(TImplementation), typeof(Service));
            services.AddScoped(typeof(IApplicationService<ENT, DTO>), typeof(Service));

            // Command handlers
            services.AddScoped(typeof(IRequestHandler<CreateCommand<ENT, DTO>, DTO>),typeof(CreateHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<UpdateCommand<ENT, DTO>, DTO>), typeof(UpdateHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<DeleteCommand<ENT, DTO>, bool>), typeof(DeleteHandler<ENT, DTO>));

            // Query handlers — disponibles automáticamente en BaseController (getById / getAll)
            services.AddScoped(typeof(IRequestHandler<GetByIdQuery<ENT, DTO>, DTO?>), typeof(GetByIdHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<GetAllQuery<ENT, DTO>, IEnumerable<DTO>>), typeof(GetAllHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<GetPaginatedQuery<ENT, DTO>, Domain.DomainShared.PaginatedList<DTO>>), typeof(GetPaginatedHandler<ENT, DTO>));
        }

    /// <summary>
    /// Registra un servicio de solo lectura (CQRS — Query side) y sus handlers genéricos.
    /// Análogo a <see cref="RegisterMediatrAbstractService{Service,DTO,ENT,TImplementation}"/>
    /// pero para <see cref="ApplicationReadOnlyService{ENT,DTO}"/>.
    /// </summary>
    public static void RegisterMediatrAbstractReadOnlyService<Service, DTO, ENT, TImplementation>(this IServiceCollection services)
            where Service : ApplicationReadOnlyService<ENT, DTO>
            where DTO : class, new()
            where ENT : class, new()
            where TImplementation : IApplicationReadOnlyService<ENT, DTO>
        {
            services.AddScoped(typeof(TImplementation), typeof(Service));
            services.AddScoped(typeof(IApplicationReadOnlyService<ENT, DTO>), typeof(Service));

            services.AddScoped(typeof(IRequestHandler<GetByIdQuery<ENT, DTO>, DTO?>),  typeof(GetByIdHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<GetAllQuery<ENT, DTO>, IEnumerable<DTO>>), typeof(GetAllHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<GetPaginatedQuery<ENT, DTO>, Domain.DomainShared.PaginatedList<DTO>>), typeof(GetPaginatedHandler<ENT, DTO>));
        }
}
