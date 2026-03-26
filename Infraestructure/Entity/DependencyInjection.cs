using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Domain.Ports;
using Infraestructure.Repository.Properties;

namespace Infraestructure.Entity;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjectionInfrastructureEf(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EntityDbContext>(options =>
            options.UseSqlServer(configuration["Sqlserver:ConnectionString"]));
            
        services.AddScoped<IEntityDbContext>(provider =>
        {
            var context = provider.GetService<EntityDbContext>();
            if (context == null)
            {
                throw new InvalidOperationException("EntityDbContext not found");
            }
            return context;
        });

        //Repository
        services.AddScoped<IPhysicalStructureRepository,PhysicalStructureRepository>();

        return services;
    }
}
