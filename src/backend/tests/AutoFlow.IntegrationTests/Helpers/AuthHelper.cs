using AutoFlow.Application.DTOs;
using System.Net.Http.Json;

namespace AutoFlow.IntegrationTests.Helpers
{
    public static class AuthHelper
    {
        public static async Task<string> LoginColaboradorAsync(HttpClient client, string email, string senha)
        {
            return await LoginAsync(client, email, senha);
        }

        public static async Task<string> LoginClienteAsync(HttpClient client, string email, string senha)
        {
            return await LoginAsync(client, email, senha);
        }

        private static async Task<string> LoginAsync(HttpClient client, string email, string senha)
        {
            var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto(email, senha));

            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return resultado!.Token;
        }
    }
}
