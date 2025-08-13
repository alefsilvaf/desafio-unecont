using Microsoft.Extensions.Logging;
using UnecontScraping.Domain;
using System.Linq; // Adicionado para IEnumarable

namespace UnecontScraping.Application
{
    public class ScrapingApplication
    {
        private readonly IScrapingService _scrapingService;
        private readonly IExportService _exportService;
        private readonly IApiService _apiService;
        private readonly ILogger<ScrapingApplication> _logger;

        public ScrapingApplication(
            IScrapingService scrapingService,
            IExportService exportService,
            IApiService apiService,
            ILogger<ScrapingApplication> logger)
        {
            _scrapingService = scrapingService;
            _exportService = exportService;
            _apiService = apiService;
            _logger = logger;
        }

        public async Task RunAsync(ScrapingConfig config)
        {
            try
            {
                _logger.LogInformation("Iniciando o processo de scraping e exportação.");

                var allBooks = await _scrapingService.ScrapeBooksAsync(config);

                var filteredBooks = allBooks.AsEnumerable();

                // Aplica a filtragem condicional
                if (config.PriceFilter.MinPrice > 0)
                {
                    _logger.LogInformation($"Aplicando filtro de preço mínimo: {config.PriceFilter.MinPrice}");
                    filteredBooks = filteredBooks.Where(b => b.Price >= config.PriceFilter.MinPrice);
                }
                
                if (config.PriceFilter.MaxPrice < decimal.MaxValue)
                {
                    _logger.LogInformation($"Aplicando filtro de preço máximo: {config.PriceFilter.MaxPrice}");
                    filteredBooks = filteredBooks.Where(b => b.Price <= config.PriceFilter.MaxPrice);
                }

                if (config.RatingFilter > 0)
                {
                    _logger.LogInformation($"Aplicando filtro de rating: {config.RatingFilter} estrelas");
                    filteredBooks = filteredBooks.Where(b => b.Rating == config.RatingFilter);
                }

                var finalBooks = filteredBooks.ToList();
                _logger.LogInformation($"Filtros aplicados. {finalBooks.Count} livros encontrados.");

                await _exportService.ExportToJsonAsync(finalBooks, "books.json");
                _logger.LogInformation("Arquivo books.json gerado com sucesso.");

                await _exportService.ExportToXmlAsync(finalBooks, "books.xml");
                _logger.LogInformation("Arquivo books.xml gerado com sucesso.");

                await _apiService.SendBooksAsync(finalBooks);

                _logger.LogInformation("Processo finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro fatal durante a execução da aplicação.");
            }
        }
    }
}