using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using UnecontScraping.Domain;

namespace UnecontScraping.Infrastructure
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendBooksAsync(List<Book> books)
        {
            _logger.LogInformation("Enviando dados para a API: https://httpbin.org/post");
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://httpbin.org/post", books);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Requisição POST enviada com sucesso. Status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Resumo da resposta da API:\n{Response}", responseContent);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao enviar dados para a API.");
            }
        }
    }
}