using RickAndMorty.Services.Storages;
using Dtos = RickAndMorty.Storage.Dtos;

namespace RickAndMorty.Services.Character.Commands
{
    public class DeleteAllCharactersCommand : IDeleteAllCharactersCommand
    {
        private readonly ICharacterStorage _characterStorage;

        public DeleteAllCharactersCommand(ICharacterStorage characterStorage)
        {
            _characterStorage = characterStorage ?? throw new ArgumentNullException(nameof(characterStorage));
        }

        public Task<bool> ExecuteAsync()
        {
            return _characterStorage.DeleteAllAsync();
        }
    }
}
