using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using UnecontScraping.Application;
using UnecontScraping.Domain;
using UnecontScraping.Infrastructure;
using UnecontScraping.Presentation;

namespace UnecontScraping.Presentation
{
    class Program
    {
        // O Main é o ponto de entrada da aplicação, responsável por configurar o DI e apresentar o menu.
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("--- Menu de Aplicação ---");
            Console.WriteLine("1. Fazer scraping dos livros");
            Console.WriteLine("2. Fazer scraping das transações");
            Console.WriteLine("0. Sair");
            Console.Write("Escolha uma opção: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await RunBookScrapingAsync(serviceProvider);
                    break;
                case "2":
                    await RunTransactionScrapingAsync(serviceProvider);
                    break;
                case "0":
                    Console.WriteLine("Encerrando a aplicação.");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Saindo...");
                    break;
            }
        }

        // ConfigureServices é o 'composition root', onde todas as dependências são registradas.
        private static void ConfigureServices(IServiceCollection services)
        {
            // Configura o carregamento do appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Mapeia a seção 'ScrapingConfig' do JSON para a classe ScrapingConfig
            services.Configure<ScrapingConfig>(configuration.GetSection("ScrapingConfig"));
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<ScrapingConfig>>().Value);

            // Configura o logger do console
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Registra os serviços para o scraping de livros
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IScrapingService, ScrapingService>();
            services.AddSingleton<ScrapingApplication>();

            // Registra os serviços para o scraping de transações
            services.AddSingleton<ITransactionScrapingService, TransactionScrapingService>();
            services.AddHttpClient<ITransactionScrapingService, TransactionScrapingService>();
            
            // Registra os serviços compartilhados
            services.AddSingleton<IExportService, ExportService>();
            services.AddHttpClient<IApiService, ApiService>();
        }
        
        // Método para executar o scraping dos livros
        private static async Task RunBookScrapingAsync(IServiceProvider serviceProvider)
        {
            var app = serviceProvider.GetRequiredService<ScrapingApplication>();
            var defaultConfig = serviceProvider.GetRequiredService<ScrapingConfig>();
            var userConfig = GetUserConfiguration(defaultConfig);
            await app.RunAsync(userConfig);
        }

        private static async Task RunTransactionScrapingAsync(IServiceProvider serviceProvider)
        {
            Console.Write("Por favor, insira o token de sessão (swsid): ");
            var swsid = Console.ReadLine();

            if (string.IsNullOrEmpty(swsid))
            {
                Console.WriteLine("Token de sessão é obrigatório. Saindo...");
                return;
            }

            var transactionService = serviceProvider.GetRequiredService<ITransactionScrapingService>();
            var transactions = await transactionService.ScrapeTransactionsAsync(swsid, 9);
            
            var exportService = serviceProvider.GetRequiredService<IExportService>();
            await exportService.ExportToJsonAsync(transactions, "transactions.json");
            Console.WriteLine("Arquivo transactions.json gerado com sucesso.");

            await exportService.ExportToXmlAsync(transactions, "transactions.xml");
            Console.WriteLine("Arquivo transactions.xml gerado com sucesso.");

            Console.WriteLine("Processo de transações finalizado com sucesso.");
        }

        private static ScrapingConfig GetUserConfiguration(ScrapingConfig defaultConfig)
        {
            Console.WriteLine("--- Configuração da Busca de Livros ---");

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