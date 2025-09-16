namespace RickAndMorty.Services.Character.Commands
{
    public interface ISaveCharacterCommand
    {
        Task<bool> ExecuteAsync(Models.Character character);
    }
}
