namespace UnecontScraping.Domain
{
    public interface ITransactionScrapingService
    {
        Task<List<Transaction>> ScrapeTransactionsAsync(string swsid, int limit);
    }
}