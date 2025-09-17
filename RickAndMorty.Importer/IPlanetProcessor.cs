using RickAndMorty.Importer.Models;
using RickAndMorty.Services.Models;

namespace RickAndMorty.Importer
{
    public interface IPlanetProcessor
    {
        Task<Planet?> GetPlanetAsync(PlanetResult planetResult);
        Task<bool> ClearPlanetsAsync();
    }
}   