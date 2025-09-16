using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Services.Character.Commands;
using RickAndMorty.Services.Character.Queries;
using RickAndMorty.Services.Planet.Commands;

namespace RickAndMorty.Storage.Sql.Extensions
{
    public static class CqrsServiceExtension
    {
        public static IServiceCollection AddCqrs(this IServiceCollection services)
        {
            services.AddScoped<IGetCharactersQuery, GetCharactersQuery>();
            services.AddScoped<ISaveCharacterCommand, SaveCharacterCommand>();
            
            services.AddScoped<ISavePlanetCommand, SavePlanetCommand>();

            return services;
        }
    }
}
