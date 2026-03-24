using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInyection
{
    public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services)
    {
        //Commands
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(configuration => 
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });
        return services;
    }
}
