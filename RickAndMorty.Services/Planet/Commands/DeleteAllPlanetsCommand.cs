using RickAndMorty.Services.Storages;

namespace RickAndMorty.Services.Planet.Commands
{
    public class DeleteAllPlanetsCommand : IDeleteAllPlanetsCommand
    {
        private readonly IPlanetStorage _planetStorage;

        public DeleteAllPlanetsCommand(IPlanetStorage planetStorage)
        {
            _planetStorage = planetStorage ?? throw new ArgumentNullException(nameof(planetStorage));
        }

        public Task<bool> ExecuteAsync()
        {
            return _planetStorage.DeleteAllAsync();
        }
    }
}
