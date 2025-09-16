using Dapper;
using RickAndMorty.Services.Data;
using RickAndMorty.Services.Models;
using RickAndMorty.Services.Storages;

namespace RickAndMorty.Storage.Sql
{
    public class CharacterStorage : ICharacterStorage
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CharacterStorage(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<Dtos.Character>?> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var sql = @"
                SELECT
                    c.Id, c.Name, c.Species, c.Type, c.Gender, c.OriginId, c.LocationId, c.Image,
                    c.DateModified, c.IsDeleted,
                    o.Name as OriginName, o.Type as OriginType, o.Dimension as OriginDimension,
                    l.Name as LocationName, l.Type as LocationType, l.Dimension as LocationDimension
                FROM Characters c
                    LEFT JOIN Planets o ON c.OriginId = o.Id
                    LEFT JOIN Planets l ON c.LocationId = l.Id
                WHERE c.IsDeleted = false;
            ";

            var dtos = await connection.QueryAsync<Dtos.Character>(sql);
            return dtos;
        }

        public async Task<bool> SaveAsync(Dtos.Character entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var dto = new Dtos.Character
            {
                Id = entity.Id,
                DateModified = DateTime.UtcNow,
                IsDeleted = false,
                Name = entity.Name,
                Species = entity.Species,
                Type = entity.Type,
                Gender = entity.Gender,
                OriginId = entity.OriginId,
                LocationId = entity.LocationId,
                Image = entity.Image,
            };

            var sql = @"
                MERGE INTO Characters AS Target
                USING (SELECT @Id AS Id) AS Source
                ON Target.Id = Source.Id
                WHEN MATCHED THEN
                    UPDATE SET
                        Name = @Name,
                        Species = @Species,
                        Type = @Type,
                        Gender = @Gender,
                        OriginId = @OriginId,
                        LocationId = @LocationId,
                        Image = @Image,
                        DateModified = @DateModified,
                        IsDeleted = @IsDeleted
                WHEN NOT MATCHED THEN
                    INSERT (Id, Name, Species, Type, Gender, OriginId, LocationId, Image, DateModified, IsDeleted)
                    VALUES (@Id, @Name, @Species, @Type, @Gender, @OriginId, @LocationId, @Image, @DateModified, @IsDeleted);";

            var affected = await connection.ExecuteAsync(sql, dto);

            return affected > 0;
        }
    }
}
