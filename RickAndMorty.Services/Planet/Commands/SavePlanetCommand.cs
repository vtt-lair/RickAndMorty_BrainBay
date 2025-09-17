using RickAndMorty.Services.Storages;
using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Planet.Commands
{
    public class SavePlanetCommand : ISavePlanetCommand
    {
        private readonly IPlanetStorage _planetStorage;

        public SavePlanetCommand(IPlanetStorage planetStorage)
        {
            _planetStorage = planetStorage ?? throw new ArgumentNullException(nameof(planetStorage));
        }

        public async Task<Models.Planet?> ExecuteAsync(Models.Planet planet)
        {
            var dto = new Dtos.Planet
            {
                Id = planet.Id,
                DateModified = DateTime.UtcNow,
                IsDeleted = false,
                Name = planet.Name,
                Type = planet.Type,
                Dimension = planet.Dimension,
            };

            var saved = await _planetStorage.SaveAsync(dto);

            if (saved) return planet;
            else return null;
        }
    }
}
