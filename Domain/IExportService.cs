namespace UnecontScraping.Domain
{
    public interface IExportService
    {
        Task ExportToJsonAsync(List<Book> books, string filePath);
        Task ExportToXmlAsync(List<Book> books, string filePath);
    }
}