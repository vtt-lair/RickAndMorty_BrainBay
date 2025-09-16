namespace RickAndMorty.Services.Planet.Commands
{
    public interface ISavePlanetCommand
    {
        Task<bool> ExecuteAsync(Models.Planet planet);
    }
}
