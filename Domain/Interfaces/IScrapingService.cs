namespace UnecontScraping.Domain
{
    public interface IScrapingService
    {
        Task<List<Book>> ScrapeBooksAsync(ScrapingConfig config);
    }
}