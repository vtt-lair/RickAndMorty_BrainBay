using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Services.Storages;

namespace RickAndMorty.Storage.Sql.Extensions
{
    public static class StorageServiceExtension
    {
        public static IServiceCollection AddSqlStorage(this IServiceCollection services)
        {
            services.AddScoped<ICharacterStorage, CharacterStorage>();
            services.AddScoped<IPlanetStorage, PlanetStorage>();            

            return services;
        }
    }
}
