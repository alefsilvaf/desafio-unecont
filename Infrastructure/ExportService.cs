using System.Text.Json;
using System.Xml.Serialization;
using UnecontScraping.Domain;

namespace UnecontScraping.Infrastructure
{
    public class ExportService : IExportService
    {
        public async Task ExportToJsonAsync<T>(List<T> data, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public async Task ExportToXmlAsync<T>(List<T> data, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, data);
                await File.WriteAllTextAsync(filePath, writer.ToString());
            }
        }
    }
}