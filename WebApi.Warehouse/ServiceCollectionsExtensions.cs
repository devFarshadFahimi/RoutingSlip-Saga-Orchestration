using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedContracts;
using System.Data;
using WebApi.Warehouse.Consumers;
using WebApi.Warehouse.Data;

namespace WebApi.Warehouse;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(p => p.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddIntegrationBus(typeof(Program).Assembly, configuration, cfg =>
        {
            cfg.AddActivity(typeof(CheckOrderItemInventoryCommandActivity));
            cfg.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
            {
                o.UseSqlServer();
                o.IsolationLevel = IsolationLevel.ReadCommitted;
                o.UseBusOutbox();
                o.QueryDelay = TimeSpan.FromSeconds(20);
            });
            cfg.AddConfigureEndpointsCallback((context, name, cfg) => cfg.UseEntityFrameworkOutbox<ApplicationDbContext>(context));
        });

        return services;
    }
}

