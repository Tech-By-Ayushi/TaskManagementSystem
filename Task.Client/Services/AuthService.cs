using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Task.Client.Auth;
using Task.Shared;

namespace Task.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);
        var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();

        if (loginResult is { Success: true, Token: not null })
        {
            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            ((JwtAuthenticationStateProvider)_authenticationStateProvider).NotifyUserAuthentication(loginResult.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);
        }

        return loginResult ?? new LoginResult { Success = false, Message = "Login failed." };
    }

    public async Task<bool> LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((JwtAuthenticationStateProvider)_authenticationStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        return true;
    }



    public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerRequest);
        return response.IsSuccessStatusCode;
    }
}
