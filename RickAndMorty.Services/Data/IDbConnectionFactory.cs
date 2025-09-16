using System.Data;

namespace RickAndMorty.Services.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}