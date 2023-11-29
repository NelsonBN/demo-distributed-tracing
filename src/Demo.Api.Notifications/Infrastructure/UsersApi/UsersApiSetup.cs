namespace Demo.Api.Notifications.Infrastructure.UsersApi;

public static class UsersApiSetup
{
    public static IServiceCollection AddUsersApi(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(UsersApiService), (sp, client) =>
        {
            var url = sp.GetRequiredService<IConfiguration>()["UsersApi"]!;
            client.BaseAddress = new Uri(url);
        });

        services.AddTransient<IUsersApiService, UsersApiService>();

        return services;
    }
}
