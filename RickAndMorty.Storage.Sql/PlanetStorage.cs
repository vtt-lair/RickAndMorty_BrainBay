using Dapper;
using RickAndMorty.Services.Data;
using RickAndMorty.Services.Storages;

namespace RickAndMorty.Storage.Sql
{
    public class PlanetStorage : IPlanetStorage
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PlanetStorage(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<Dtos.Planet>?> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var dtos = await connection.QueryAsync<Dtos.Planet>("SELECT Id, Name, Type, Dimension FROM Planets;");
            return dtos;
        }

        public async Task<bool> SaveAsync(Dtos.Planet entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var dto = new Dtos.Planet
            {
                Id = entity.Id,
                DateModified = DateTime.UtcNow,
                IsDeleted = false,
                Name = entity.Name,
                Type = entity.Type,
                Dimension = entity.Dimension,
            };

            var sql = @"
                MERGE INTO Planets AS Target
                USING (SELECT @Id AS Id) AS Source
                ON Target.Id = Source.Id
                WHEN MATCHED THEN
                    UPDATE SET
                        Name = @Name,
                        Type = @Type,
                        Dimension = @Dimension,
                        DateModified = @DateModified,
                        IsDeleted = @IsDeleted
                WHEN NOT MATCHED THEN
                    INSERT (Id, Name, Type, Dimension, DateModified, IsDeleted)
                    VALUES (@Id, @Name, @Type, @Dimension, @DateModified, @IsDeleted);";

            var affected = await connection.ExecuteAsync(sql, dto);

            return affected > 0;
        }
    }
}
