using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Importer;

namespace RickAndMorty.Storage.Sql.Extensions
{
    public static class ProcessorServiceExtension
    {
        public static IServiceCollection AddProcessors(this IServiceCollection services)
        {
            services.AddTransient<IPlanetProcessor, PlanetProcessor>();
            services.AddTransient<ICharacterProcessor, CharacterProcessor>();

            return services;
        }
    }
}
