using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using BlazingBooks.Data;

namespace BlazingBooks.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _sessionStorage = sessionStorage;
        }

        public async Task<RegisterResult> Register(UserData userData)
        {
            var registerAsJson = JsonSerializer.Serialize(userData);
            var response = await _httpClient.PostAsync("http://localhost:20845/api/Auth/register", new StringContent(registerAsJson, Encoding.UTF8, "application/json"));
            return JsonSerializer.Deserialize<RegisterResult>(await response.Content.ReadAsStringAsync());
        }

        public async Task<LoginResult> Login(UserData userData)
        {
            var loginAsJson = JsonSerializer.Serialize(userData);
            var response = await _httpClient.PostAsync("http://localhost:20845/api/Auth/login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await _sessionStorage.SetAsync("authToken", loginResult.Token);
            ((ApiAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await _sessionStorage.DeleteAsync("authToken");
            ((ApiAuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
