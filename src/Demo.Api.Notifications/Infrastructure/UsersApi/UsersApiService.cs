namespace Demo.Api.Notifications.Infrastructure.UsersApi;

public interface IUsersApiService
{
    Task<UserResponse?> GetUserAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class UsersApiService(IHttpClientFactory HttpFactory) : IUsersApiService
{
    private readonly IHttpClientFactory _httpFactory = HttpFactory;

    public async Task<UserResponse?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var client = _httpFactory.CreateClient(nameof(UsersApiService));

        var response = await client.GetFromJsonAsync<UserResponse>($"users/{id}", cancellationToken);

        return response;
    }
}
