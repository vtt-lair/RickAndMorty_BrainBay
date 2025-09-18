using RickAndMorty.Services.Storages;

namespace RickAndMorty.Services.Character.Queries
{
    public class GetPlanetsQuery : IGetPlanetsQuery
    {
        private readonly IPlanetStorage _planetStorage;

        public GetPlanetsQuery(IPlanetStorage planetStorage)
        {
            _planetStorage = planetStorage ?? throw new ArgumentNullException(nameof(planetStorage)); ;
        }

        public async Task<IEnumerable<Models.Planet>?> ExecuteAsync()
        {
            var dtos = await _planetStorage.GetAllAsync();
            if (dtos == null) return null;

            return dtos.Select(dto => new Models.Planet
            {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.Type,
                Dimension = dto.Dimension,
            });
        }
    }
}
