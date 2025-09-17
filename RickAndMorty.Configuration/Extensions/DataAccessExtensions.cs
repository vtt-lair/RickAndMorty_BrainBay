using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RickAndMorty.Services.Data;
using RickAndMorty.Storage.Sql;

namespace RickAndMorty.Configuration.Extensions
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = ConnectionStringProvider.GetConnectionString(configuration);
            services.AddSingleton<IDbConnectionFactory>(new SqlServerConnectionFactory(connectionString));
            
            return services;
        }
    }
}