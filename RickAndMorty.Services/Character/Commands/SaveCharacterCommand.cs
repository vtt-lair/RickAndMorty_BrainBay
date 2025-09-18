using RickAndMorty.Services.Storages;
using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Character.Commands
{
    public class SaveCharacterCommand : ISaveCharacterCommand
    {
        private readonly ICharacterStorage _characterStorage;

        public SaveCharacterCommand(ICharacterStorage characterStorage)
        {
            _characterStorage = characterStorage ?? throw new ArgumentNullException(nameof(characterStorage));
        }

        public async Task<Models.Character?> ExecuteAsync(Models.Character character)
        {
            var dto = new Dtos.Character
            {
                DateModified = DateTime.UtcNow,
                IsDeleted = false,
                Name = character.Name,
                Species = character.Species,
                Type = character.Type,
                Gender = character.Gender,
                OriginId = character.OriginId,
                LocationId = character.LocationId,
                Image = character.Image,
            };

            var saved = await _characterStorage.SaveAsync(dto);
            if (saved) return character;
            else return null;
        }
    }
}
