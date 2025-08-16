using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnecontScraping.Domain;

namespace UnecontScraping.Infrastructure
{
    public class TransactionScrapingService : ITransactionScrapingService
    {
        private readonly ILogger<TransactionScrapingService> _logger;
        private readonly HttpClient _httpClient;

        public TransactionScrapingService(ILogger<TransactionScrapingService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<List<Transaction>> ScrapeTransactionsAsync(string swsid, int limit)
        {
            var allTransactions = new List<Transaction>();
            var skip = 0;
            var hasMoreTransactions = true;

            _httpClient.DefaultRequestHeaders.Add("swsid", swsid);

            _logger.LogInformation("Iniciando scraping das transações via API. Limite por página: {Limit}", limit);

            while (hasMoreTransactions)
            {
                var apiUrl = $"https://rest.minimania.app/user/transactions?skip={skip}&limit={limit}";
                _logger.LogDebug("Requisitando página com skip={Skip}", skip);

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var apiResponse = await response.Content.ReadFromJsonAsync<TransactionApiResponse>();
                
                // Nova condição de parada mais robusta
                if (apiResponse?.Transactions == null || apiResponse.Transactions.Count == 0 || apiResponse.Transactions.Count < limit)
                {
                    hasMoreTransactions = false;
                }
                
                if (apiResponse?.Transactions != null)
                {
                    allTransactions.AddRange(apiResponse.Transactions);
                }

                if(hasMoreTransactions)
                {
                    skip += limit;
                }
            }
            
            _logger.LogInformation("Fim das transações. Total de registros: {Total}", allTransactions.Count);
            return allTransactions;
        }
    }
}