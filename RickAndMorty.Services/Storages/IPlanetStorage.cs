using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Storages
{
    public interface IPlanetStorage
    {
        public Task<IEnumerable<Dtos.Planet>?> GetAllAsync();
    }
}
