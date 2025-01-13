using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApi.SagaOrchestration.Consumers;
using WebApi.SagaOrchestration.DataLayer;
using WebApi.SagaOrchestration.StateMachines;

namespace WebApi.SagaOrchestration;

public static class ServiceCollections
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEndpointAddressProvider, DbEndpointAddressProvider>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.AddConsumers(typeof(ProcessSettleRequestEventConsumer).Assembly);
            cfg.AddMediator(p =>
            {
                p.AddConsumers(typeof(ProcessSettleRequestEventConsumer).Assembly);
            });

            cfg.AddSagaStateMachine<SettlementStateMachine, SettlementState>(typeof(SettlementStateMachineDefinition))
            .EntityFrameworkRepository(opt =>
            {
                opt.AddDbContext<DbContext, SagaContext>((provider, builder) =>
                {
                    builder.UseSqlServer(connectionString, m => m.MigrationsAssembly(typeof(ProcessSettleRequestEventConsumer).Assembly));
                });
            });

            cfg.AddEntityFrameworkOutbox<SagaContext>(o =>
            {
                o.UseSqlServer();
                o.IsolationLevel = IsolationLevel.ReadCommitted;
                o.UseBusOutbox();
                o.QueryDelay = TimeSpan.FromSeconds(20);
            });

            cfg.UsingRabbitMq((context, config) =>
            {
                //config.PrefetchCount = 20;
                //config.ConcurrentMessageLimit = 100;
                config.Host(configuration.GetSection("RabbitConfig:host").Value, h =>
                {
                    h.Username(configuration.GetSection("RabbitConfig:username").Value!);
                    h.Password(configuration.GetSection("RabbitConfig:password").Value!);
                });
                config.UseDelayedMessageScheduler();
                config.ConfigureEndpoints(context);
            });

            cfg.AddDelayedMessageScheduler();
            //cfg.AddConfigureEndpointsCallback((context, name, cfgg) => cfgg.UseEntityFrameworkOutbox<SagaContext>(context));
            //cfg.AddConfigureEndpointsCallback((context, _, cage) =>
            //{
            //    // cage.UseDelayedRedelivery(r =>
            //    // {
            //    //     r.Handle<SagaRetryException>();
            //    //     r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(30));
            //    // });


            //    //  cage.UseEntityFrameworkOutbox<LoanPaymentContext>(context);
            //});


        });

        services.AddDbContext<SagaContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}