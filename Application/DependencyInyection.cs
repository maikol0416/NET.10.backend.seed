using System.Reflection;
using Application.Base;
using Application.Dto;
using Application.Service;
using Application.Validator;
using Domain.BoundedContext.Properties;
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
        return services;
    }

    public static void RegisterValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<PhysicalStructureDto>, PhysicalStructureValidator>();
    }

    public static void RegisterMediatrAbstractService<Service, DTO, ENT, TImplementation>(this IServiceCollection services)
            where Service : ApplicationService<ENT, DTO>
            where DTO : class, new()
            where ENT : class, new()
            where TImplementation : IApplicationService<ENT, DTO>
        {
            services.AddScoped(typeof(TImplementation), typeof(Service));
            services.AddScoped(typeof(IApplicationService<ENT, DTO>), typeof(Service));

            services.AddScoped(typeof(IRequestHandler<CreateCommand<ENT, DTO>, DTO>),typeof(CreateHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<UpdateCommand<ENT, DTO>, DTO>), typeof(UpdateHandler<ENT, DTO>));
            services.AddScoped(typeof(IRequestHandler<DeleteCommand<ENT, DTO>, bool>), typeof(DeleteHandler<ENT, DTO>));
        }
}
