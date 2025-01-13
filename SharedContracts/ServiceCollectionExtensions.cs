using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedContracts.Services;
using System.Reflection;

namespace SharedContracts;
public static class ServiceCollectionExtensions
{
    public static void AddIntegrationBus(this IServiceCollection services, Assembly assembly, IConfiguration configuration, Action<IBusRegistrationConfigurator> configure)
    {
        services.AddScoped<IIntegrationBus, IntegrationBus>();
        services.AddMassTransit(configure);

        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.AddConsumers(assembly);

            cfg.AddDelayedMessageScheduler();
            cfg.UsingRabbitMq((context, cfgg) =>
            {
                cfgg.UseDelayedMessageScheduler();
                cfgg.ConcurrentMessageLimit = 100;
                cfgg.ConfigureEndpoints(context);
                cfgg.Host(configuration.GetSection("RabbitConfig:host").Value, h =>
                {
                    h.Username(configuration.GetSection("RabbitConfig:username").Value!);
                    h.Password(configuration.GetSection("RabbitConfig:password").Value!);
                });
            });

            configure(cfg);
        });
    }
}
