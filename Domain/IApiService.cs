namespace UnecontScraping.Domain
{
    public interface IApiService
    {
        Task SendBooksAsync(List<Book> books);
    }
}