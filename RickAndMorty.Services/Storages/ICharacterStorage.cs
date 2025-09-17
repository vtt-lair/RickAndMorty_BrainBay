using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Storages
{
    public interface ICharacterStorage
    {
        public Task<IEnumerable<Dtos.Character>?> GetAllAsync();

        public Task<bool> SaveAsync(Dtos.Character entity);

        public Task<bool> BulkSaveAsync(IEnumerable<Dtos.Character> entities);

        public Task<bool> DeleteAllAsync();

    }
}
