namespace RickAndMorty.Services.Character.Commands
{
    public interface IBulkSaveCharacterCommand
    {
        Task<bool> ExecuteAsync(IEnumerable<Models.Character> characters);
    }
}
