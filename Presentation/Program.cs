using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnecontScraping.Application;
using UnecontScraping.Domain;
using UnecontScraping.Infrastructure;

namespace UnecontScraping.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetRequiredService<ScrapingApplication>();
            var config = serviceProvider.GetRequiredService<ScrapingConfig>();

            await app.RunAsync(config);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuração
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.Configure<ScrapingConfig>(configuration.GetSection("ScrapingConfig"));
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<ScrapingConfig>>().Value);

            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Injeção de Dependência
            services.AddSingleton<IScrapingService, ScrapingService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddHttpClient<IApiService, ApiService>();
            services.AddSingleton<ScrapingApplication>();
        }
    }
}