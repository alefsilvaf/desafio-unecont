using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using UnecontScraping.Application;
using UnecontScraping.Domain;
using UnecontScraping.Infrastructure;
using UnecontScraping.Presentation;

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
            
            var defaultConfig = serviceProvider.GetRequiredService<ScrapingConfig>();

            var userConfig = GetUserConfiguration(defaultConfig);

            await app.RunAsync(userConfig);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.Configure<ScrapingConfig>(configuration.GetSection("ScrapingConfig"));
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<ScrapingConfig>>().Value);

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IScrapingService, ScrapingService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddHttpClient<IApiService, ApiService>();
            services.AddSingleton<ScrapingApplication>();
        }

        private static ScrapingConfig GetUserConfiguration(ScrapingConfig defaultConfig)
        {
            Console.WriteLine("--- Configuração da Busca ---");
            
            Console.WriteLine($"Categorias (padrão: {string.Join(", ", defaultConfig.Categories)}), separe por vírgulas: ");
            var categoriesInput = Console.ReadLine();
            var categories = string.IsNullOrEmpty(categoriesInput)
                ? defaultConfig.Categories
                : categoriesInput.Split(',').Select(c => c.Trim()).ToArray();

            Console.WriteLine($"Preço Mínimo (padrão: {defaultConfig.PriceFilter?.MinPrice}), deixe em branco para não filtrar: ");
            var minPriceInput = Console.ReadLine();
            decimal.TryParse(minPriceInput, out var minPriceValue);
            decimal? minPrice = string.IsNullOrEmpty(minPriceInput) ? null : minPriceValue;
            
            Console.WriteLine($"Preço Máximo (padrão: {defaultConfig.PriceFilter?.MaxPrice}), deixe em branco para não filtrar: ");
            var maxPriceInput = Console.ReadLine();
            decimal.TryParse(maxPriceInput, out var maxPriceValue);
            decimal? maxPrice = string.IsNullOrEmpty(maxPriceInput) ? null : maxPriceValue;

            Console.WriteLine($"Número de Estrelas (1-5, padrão: {defaultConfig.RatingFilter}), deixe em branco para não filtrar: ");
            var ratingInput = Console.ReadLine();
            int.TryParse(ratingInput, out var ratingValue);
            int? rating = string.IsNullOrEmpty(ratingInput) ? null : ratingValue;
            
            Console.WriteLine("-----------------------------");

            return new ScrapingConfig
            {
                Categories = categories,
                PriceFilter = new PriceFilter
                {
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                },
                RatingFilter = rating
            };
        }
    }
}