namespace RickAndMorty.Services.Character.Queries
{
    public interface IGetPlanetsQuery : IQuery
    {
        public Task<IEnumerable<Models.Planet>?> ExecuteAsync();
    }
}
