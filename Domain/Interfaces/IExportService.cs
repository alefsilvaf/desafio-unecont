namespace UnecontScraping.Domain
{
    public interface IExportService
    {
        Task ExportToJsonAsync<T>(List<T> data, string filePath);
        Task ExportToXmlAsync<T>(List<T> data, string filePath);
    }
}