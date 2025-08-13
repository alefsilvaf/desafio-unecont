using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using UnecontScraping.Domain;

namespace UnecontScraping.Infrastructure
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly HttpClient _httpClient;

        public CategoryService(ILogger<CategoryService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, int>> GetCategoriesAsync()
        {
            var categories = new Dictionary<string, int>();
            var url = "https://books.toscrape.com/";
            _logger.LogInformation("Iniciando scraping das categorias a partir da URL: {Url}", url);

            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var categoryNodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='nav nav-list']/li/ul/li/a");

                if (categoryNodes == null)
                {
                    _logger.LogError("Não foi possível encontrar a lista de categorias na página inicial.");
                    return categories;
                }

                var regex = new Regex(@"_(\d+)");

                foreach (var node in categoryNodes)
                {
                    var href = node.GetAttributeValue("href", "");
                    var name = node.InnerText.Trim();
                    var match = regex.Match(href);

                    if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
                    {
                        categories[name.ToLower().Replace(" ", "-")] = id;
                        _logger.LogDebug("Categoria encontrada: '{Name}' com ID '{Id}'", name, id);
                    }
                }

                _logger.LogInformation("{Count} categorias encontradas.", categories.Count);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao acessar a URL para obter as categorias.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar as categorias.");
            }

            return categories;
        }
    }
}