namespace Demo.Api.Notifications.Infrastructure.Database;

public static class DatabaseSetup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>()
                .AddScoped<INotificationsRepository, NotificationsRepository>();

        return services;
    }
}
