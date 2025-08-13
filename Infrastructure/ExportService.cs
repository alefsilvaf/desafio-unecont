using System.Text.Json;
using System.Xml.Serialization;
using UnecontScraping.Domain;

namespace UnecontScraping.Infrastructure
{
    public class ExportService : IExportService
    {
        public async Task ExportToJsonAsync(List<Book> books, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(books, options);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public async Task ExportToXmlAsync(List<Book> books, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Book>));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, books);
                await File.WriteAllTextAsync(filePath, writer.ToString());
            }
        }
    }
}