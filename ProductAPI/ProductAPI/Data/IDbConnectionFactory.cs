using System.Data;

namespace ProductAPI.Data
{

    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}