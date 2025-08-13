using Microsoft.Extensions.Logging;
using UnecontScraping.Domain;

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

                var filteredBooks = allBooks
                    .Where(b => b.Price >= config.PriceFilter.MinPrice && b.Price <= config.PriceFilter.MaxPrice)
                    .Where(b => b.Rating == config.RatingFilter)
                    .ToList();

                _logger.LogInformation("Filtros aplicados. {Count} livros encontrados.", filteredBooks.Count);

                await _exportService.ExportToJsonAsync(filteredBooks, "books.json");
                _logger.LogInformation("Arquivo books.json gerado com sucesso.");

                await _exportService.ExportToXmlAsync(filteredBooks, "books.xml");
                _logger.LogInformation("Arquivo books.xml gerado com sucesso.");

                await _apiService.SendBooksAsync(filteredBooks);

                _logger.LogInformation("Processo finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro fatal durante a execução da aplicação.");
            }
        }
    }
}