using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Services.Data;
using RickAndMorty.Services.Storages;

namespace RickAndMorty.Storage.Sql.Extensions
{
    public static class StorageServiceExtension
    {
        public static IServiceCollection AddSqlStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(connectionString));

            services.AddScoped<ICharacterStorage, CharacterStorage>();
            services.AddScoped<IPlanetStorage, PlanetStorage>();            

            return services;
        }
    }
}
