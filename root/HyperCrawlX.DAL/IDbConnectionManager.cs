using Npgsql;

namespace HyperCrawlX.DAL
{
    public interface IDbConnectionManager
    {
        Task<NpgsqlConnection> CreateConnection();
    }
}
