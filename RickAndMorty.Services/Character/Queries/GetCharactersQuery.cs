using RickAndMorty.Services.Storages;

namespace RickAndMorty.Services.Character.Queries
{
    public class GetCharactersQuery : IGetCharactersQuery
    {
        private readonly ICharacterStorage _characterStorage;

        public GetCharactersQuery(ICharacterStorage characterStorage)
        {
            _characterStorage = characterStorage ?? throw new ArgumentNullException(nameof(characterStorage)); ;
        }

        public async Task<IEnumerable<Models.Character>?> ExecuteAsync()
        {
            var dtos = await _characterStorage.GetAllAsync();
            if (dtos == null) return null;

            return dtos.Select(dto => new Models.Character
            {
                Id = dto.Id,
                ExternalId = dto.ExternalId,
                Name = dto.Name,
                Species = dto.Species,
                Type = dto.Type,
                Gender = dto.Gender,
                OriginId = dto.OriginId,
                Origin = dto.OriginId.HasValue ? new Models.Planet
                {
                    Id = dto.OriginId.Value,
                    Name = dto.OriginName ?? "",
                    Type = dto.OriginType,
                    Dimension = dto.OriginDimension
                } : null,
                LocationId = dto.LocationId,
                Location = dto.LocationId.HasValue ? new Models.Planet
                {
                    Id = dto.LocationId.Value,
                    Name = dto.LocationName ?? "",
                    Type = dto.LocationType,
                    Dimension = dto.LocationDimension
                } : null,
                Image = dto.Image
            });
        }
    }
}
