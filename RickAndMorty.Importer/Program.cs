using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RickAndMorty.Importer.Configuration;

namespace RickAndMorty.Importer
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Rick and Morty Import");

            try
            {
                ConfigureServices();
                await ClearDataAsync();
                await GetDataAsync();
                Console.WriteLine("Import completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import failed: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
                .Build();

            services.AddSingleton(provider =>
            {
                return new ApiConfiguration
                {
                    RemoteApi = new Uri(config["RemoteAPI"] ?? throw new InvalidOperationException("RemoteAPI configuration is missing")),
                    LocalApi = new Uri(config["LocalAPI"] ?? throw new InvalidOperationException("LocalAPI configuration is missing"))
                };
            });

            services.AddHttpClient();

            services.AddLogging(builder => builder.AddConsole());

            services.AddTransient<CharacterProcessor>();
            services.AddTransient<PlanetProcessor>();

            _serviceProvider = services.BuildServiceProvider();
        }

        private static async Task ClearDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var characterProcessor = scope.ServiceProvider.GetRequiredService<CharacterProcessor>();
            var planetProcessor = scope.ServiceProvider.GetRequiredService<PlanetProcessor>();

            await characterProcessor.ClearCharactersAsync();
            await planetProcessor.ClearPlanetsAsync();
        }

        private static async Task GetDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var characterProcessor = scope.ServiceProvider.GetRequiredService<CharacterProcessor>();

            var result = await characterProcessor.GetCharactersAsync();
            await characterProcessor.ProcessResultsAsync(result);
        }
    }
}
