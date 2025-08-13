using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using UnecontScraping.Domain;

namespace UnecontScraping.Infrastructure
{
    public class ScrapingService : IScrapingService
    {
        private readonly ILogger<ScrapingService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ICategoryService _categoryService;

        public ScrapingService(ILogger<ScrapingService> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _categoryService = categoryService;
        }

        public async Task<List<Book>> ScrapeBooksAsync(ScrapingConfig config)
        {
            var books = new List<Book>();
            var categoryIds = await _categoryService.GetCategoriesAsync();
            
            _logger.LogInformation("Iniciando o scraping das categorias configuradas: {Categories}", string.Join(", ", config.Categories));

            foreach (var category in config.Categories)
            {
                if (categoryIds.TryGetValue(category, out var categoryId))
                {
                    var url = $"https://books.toscrape.com/catalogue/category/books/{category}_{categoryId}/index.html";
                    _logger.LogInformation("Scraping da categoria '{Category}' iniciado. URL: {Url}", category, url);
                    await ScrapeCategoryAsync(url, category, books);
                }
                else
                {
                    _logger.LogWarning("Categoria '{Category}' não encontrada no mapeamento dinâmico. Pulando...", category);
                }
            }

            return books;
        }

        private async Task ScrapeCategoryAsync(string url, string category, List<Book> books)
        {
            var nextPageUrl = url;
            while (!string.IsNullOrEmpty(nextPageUrl))
            {
                try
                {
                    _logger.LogDebug("Baixando página: {Url}", nextPageUrl);
                    var html = await _httpClient.GetStringAsync(nextPageUrl);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var bookNodes = htmlDoc.DocumentNode.SelectNodes("//article[@class='product_pod']");
                    if (bookNodes == null)
                    {
                        _logger.LogWarning("Nenhum livro encontrado na página: {Url}", nextPageUrl);
                        break;
                    }

                    foreach (var node in bookNodes)
                    {
                        var title = node.SelectSingleNode(".//h3/a").GetAttributeValue("title", "");
                        var priceText = node.SelectSingleNode(".//p[@class='price_color']").InnerText;
                        var ratingText = node.SelectSingleNode(".//p[contains(@class, 'star-rating')]").GetAttributeValue("class", "");
                        var bookUrl = node.SelectSingleNode(".//h3/a").GetAttributeValue("href", "");

                        var book = new Book
                        {
                            Title = title,
                            Price = ConvertToDecimal(priceText),
                            Rating = ConvertToRating(ratingText),
                            Category = category,
                            Url = new Uri(new Uri(nextPageUrl), bookUrl).ToString()
                        };

                        books.Add(book);
                        _logger.LogDebug("Livro extraído: {Title}", book.Title);
                    }

                    var nextNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='next']/a");
                    if (nextNode != null)
                    {
                        var relativeUrl = nextNode.GetAttributeValue("href", "");
                        nextPageUrl = new Uri(new Uri(url), relativeUrl).ToString();
                    }
                    else
                    {
                        nextPageUrl = null;
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Erro ao acessar a URL {Url}", nextPageUrl);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao processar a página {Url}", nextPageUrl);
                    break;
                }
            }
        }

        private decimal ConvertToDecimal(string priceText)
        {
            priceText = Regex.Replace(priceText, "[^0-9.]", "");
            decimal.TryParse(priceText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var price);
            return price;
        }

        private int ConvertToRating(string ratingText)
        {
            return ratingText.ToLower().Contains("one") ? 1 :
                   ratingText.ToLower().Contains("two") ? 2 :
                   ratingText.ToLower().Contains("three") ? 3 :
                   ratingText.ToLower().Contains("four") ? 4 :
                   ratingText.ToLower().Contains("five") ? 5 : 0;
        }
    }
}