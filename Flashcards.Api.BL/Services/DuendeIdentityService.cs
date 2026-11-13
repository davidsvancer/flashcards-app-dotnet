using Flashcards.Api.BL.Services.Interfaces;
using Flashcards.Common.Options;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Flashcards.Api.BL.Services
{
    public class DuendeIdentityService(HttpClient httpClient, IOptions<IdentityOptions> options)
        : IIdentityService
    {
        private TokenResponse? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;
        private readonly IdentityOptions _options = options.Value;

        private async Task<string> GetAccessTokenAsync()
        {
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken.AccessToken!;
            }

            // Validating that backend config exists before trying to use it
            if (string.IsNullOrEmpty(_options.BackendClientId) ||
                string.IsNullOrEmpty(_options.BackendClientSecret))
            {
                throw new InvalidOperationException("Backend Identity Configuration (ClientId/Secret) is missing.");
            }

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = $"{_options.Authority}/connect/token",
                    ClientId = _options.BackendClientId,
                    ClientSecret = _options.BackendClientSecret,
                    Scope = _options.UserManagementScope
                });

            if (tokenResponse.IsError)
            {
                throw new InvalidOperationException(
                    $"Failed to get access token: {tokenResponse.Error} - {tokenResponse.ErrorDescription}");
            }

            _cachedToken = tokenResponse;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);

            return tokenResponse.AccessToken!;
        }

        public async Task<string> CreateIdentityAsync(string username, string password, string role)
        {
            // Prepare the payload required by your Duende Controller
            var requestBody = new
            {
                Username = username,
                Password = password,
                Subject = username,
                Email = $"{username}@email.com",
                Role = role
            };

            var response = await httpClient.PostAsJsonAsync($"{_options.Authority}/api/user", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Identity Creation Failed: {response.StatusCode} - {errorDetails}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return result.Trim('"');
        }

        public async Task DeleteIdentityAsync(string userId)
        {

            var response = await httpClient.DeleteAsync($"{_options.Authority}/api/user/{userId}");

            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Failed to delete user: {response.StatusCode} - {error}");
            }
        }
    }
}