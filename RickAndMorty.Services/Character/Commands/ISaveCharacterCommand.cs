namespace RickAndMorty.Services.Character.Commands
{
    public interface ISaveCharacterCommand
    {
        Task<Models.Character?> ExecuteAsync(Models.Character character);
    }
}
