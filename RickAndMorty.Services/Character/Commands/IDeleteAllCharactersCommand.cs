namespace RickAndMorty.Services.Character.Commands
{
    public interface IDeleteAllCharactersCommand
    {
        Task<bool> ExecuteAsync();
    }
}
