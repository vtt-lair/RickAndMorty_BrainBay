using RickAndMorty.Services.Storages;
using System.Threading.Tasks;
using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Character.Commands
{
    public class BulkSaveCharacterCommand : IBulkSaveCharacterCommand
    {
        private readonly ICharacterStorage _characterStorage;

        public BulkSaveCharacterCommand(ICharacterStorage characterStorage)
        {
            _characterStorage = characterStorage ?? throw new ArgumentNullException(nameof(characterStorage));
        }

        public async Task<bool> ExecuteAsync(IEnumerable<Models.Character> characters)
        {
            var dtos = characters.Select(entity => new Dtos.Character
            {
                Id = entity.Id,
                DateModified = DateTime.UtcNow,
                IsDeleted = false,
                Name = entity.Name,
                Species = entity.Species,
                Type = entity.Type,
                Gender = entity.Gender,
                OriginId = entity.OriginId,
                LocationId = entity.LocationId,
                Image = entity.Image,
            });

            return await _characterStorage.BulkSaveAsync(dtos);
        }
    }
}
