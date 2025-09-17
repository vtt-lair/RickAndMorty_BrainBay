using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RickAndMorty.Configuration;
using RickAndMorty.Importer.Configuration;
using RickAndMorty.Storage.Sql.Extensions;
using RickAndMorty.Storage.Migrator.Extensions;
using RickAndMorty.Configuration.Extensions;

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
                await RunMigrationsAsync();
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
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.api.json"), optional: true)
                .Build();

            services.AddDataAccess(config);
            services.AddMigrations(config);
            services.AddProcessors();

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

            _serviceProvider = services.BuildServiceProvider();
        }

        private static async Task RunMigrationsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<FluentMigrator.Runner.IMigrationRunner>();
            Console.WriteLine("Running database migrations...");
            runner.MigrateUp();
            Console.WriteLine("Database migrations completed.");
        }

        private static async Task ClearDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var characterProcessor = scope.ServiceProvider.GetRequiredService<ICharacterProcessor>();
            var planetProcessor = scope.ServiceProvider.GetRequiredService<IPlanetProcessor>();

            await characterProcessor.ClearCharactersAsync();
            await planetProcessor.ClearPlanetsAsync();
        }

        private static async Task GetDataAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var characterProcessor = scope.ServiceProvider.GetRequiredService<ICharacterProcessor>();

            var result = await characterProcessor.GetCharactersAsync();
            await characterProcessor.ProcessResultsAsync(result);
        }
    }
}
