using RabbitMQ.Client;

namespace Demo.Api.Notifications.Infrastructure.MessageBroker;

public static class MessageBusSetup
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services
            .ConfigureOptions<MessageBusOptions.Setup>()
            .AddSingleton<IConnectionFactory>(sp => sp.GetRequiredService<IConfiguration>().GetSection(MessageBusOptions.Setup.SECTION_NAME).Get<ConnectionFactory>()!)
            .AddTransient<IMessageBus, MessageBus>();

        services
            .AddHostedService<MessageBusSetupWorker>()
            .AddHostedService<MessageBusConsumer>();

        return services;
    }
}
