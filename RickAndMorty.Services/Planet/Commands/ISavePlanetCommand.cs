namespace RickAndMorty.Services.Planet.Commands
{
    public interface ISavePlanetCommand
    {
        Task<Models.Planet> ExecuteAsync(Models.Planet planet);
    }
}
