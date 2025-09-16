namespace RickAndMorty.Services.Character.Queries
{
    public interface IGetCharactersQuery : IQuery
    {
        public Task<IEnumerable<Models.Character>?> ExecuteAsync();
    }
}
