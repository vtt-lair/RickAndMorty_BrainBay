using Microsoft.Extensions.Configuration;

namespace RickAndMorty.Configuration
{
    public static class ConnectionStringProvider
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DefaultConnection string is not configured.");
            }
            return connectionString;
        }
    }
}