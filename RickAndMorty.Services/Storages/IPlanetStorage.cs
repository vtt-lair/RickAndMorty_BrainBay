using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Storages
{
    public interface IPlanetStorage
    {
        public Task<IEnumerable<Dtos.Planet>?> GetAllAsync();

        public Task<bool> SaveAsync(Dtos.Planet entity);
    }
}
