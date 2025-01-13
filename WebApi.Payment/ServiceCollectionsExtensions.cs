using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedContracts;
using System.Data;
using WebApi.Payment.Consumers;
using WebApi.Payment.Data;

namespace WebApi.Payment;

public static class ServiceCollectionsExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(p => p.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIntegrationBus(typeof(Program).Assembly, configuration, cfg =>
        {
            cfg.AddActivity(typeof(CheckingForPaymentStatusCommandActivity));
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

