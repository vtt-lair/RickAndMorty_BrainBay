using Dapper;
using RickAndMorty.Services.Data;
using RickAndMorty.Services.Models;
using RickAndMorty.Services.Storages;

using Dtos = RickAndMorty.Storage.Dtos;

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
    }
}
