using System.Net.Http.Json;
using Task.Shared;

namespace Task.Client.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserDto>?> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("api/users");
    }
}