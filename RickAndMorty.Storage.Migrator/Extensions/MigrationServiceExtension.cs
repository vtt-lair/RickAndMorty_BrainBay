using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Storage.Migrator.Migrations;

namespace RickAndMorty.Storage.Migrator.Extensions
{
    public static class MigrationServiceExtension
    {
        public static IServiceCollection AddMigrations(this IServiceCollection services, string connectionString)
        {
            EnsureDatabaseExists(connectionString);

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(CreateCharacterTable).Assembly).For.All())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            return services;
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var dbName = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            var masterConnectionString = builder.ConnectionString;

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                var sql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}') CREATE DATABASE [{dbName}]";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}