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

            var dtos = await connection.QueryAsync<Dtos.Planet>(@"
                SELECT Id, Name, Type, 
                CASE 
                    WHEN Dimension IS NULL OR 
                         Dimension = '' OR 
                         LOWER(TRIM(Dimension)) IN ('unknown', 'unknown dimension')
                    THEN 'values.unknown_dimension' 
                    ELSE Dimension 
                END AS Dimension 
                FROM Planets 
                ORDER BY Dimension, Name;");
            return dtos;
        }

        public async Task<bool> SaveAsync(Dtos.Planet entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

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

            var affected = await connection.ExecuteAsync(sql, entity);

            return affected > 0;
        }

        public async Task<bool> DeleteAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
         
            var sql = "DELETE FROM Planets;";
            var affected = await connection.ExecuteAsync(sql);
            
            return affected > 0;
        }
    }
}
