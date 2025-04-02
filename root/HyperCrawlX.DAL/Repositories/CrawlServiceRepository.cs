using Dapper;
using HyperCrawlX.DAL.Interfaces;
using HyperCrawlX.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HyperCrawlX.DAL.Repositories
{
    public class CrawlServiceRepository : ICrawlServiceRepository
    {
        private readonly ILogger<CrawlServiceRepository> _logger;
        private readonly IDbConnectionManager _dbConnectionManager;

        public CrawlServiceRepository(
            ILogger<CrawlServiceRepository> logger, 
            IDbConnectionManager dbConnectionManager)
        {
            _logger = logger;
            _dbConnectionManager = dbConnectionManager;
        }

        public async void UpdateRequestStatus(long requestId, int status)
        {
            _logger.LogInformation($"CrawlServiceRepository - Updating status to {status} for request: {requestId}");

            // Define the SQL query to execute
            string sql = @"UPDATE CRAWL_REQUEST SET Status = @status WHERE RequestId = @requestId";

            // Define the parameters to pass to the query
            DynamicParameters dynamicParams = new DynamicParameters();
            dynamicParams.Add("requestId", requestId, DbType.Int64, ParameterDirection.Input);
            dynamicParams.Add("status", status, DbType.Int32, ParameterDirection.Input);

            // Create connection and execute the query
            using IDbConnection conn = _dbConnectionManager.CreateConnection();
            await conn.ExecuteAsync(sql, dynamicParams);

            _logger.LogInformation("CrawlServiceRepository - Updated crawl request status");
            return;
        }

        public async Task<CrawlRequest?> GetPendingCrawlRequest()
        {
            _logger.LogInformation($"CrawlServiceRepository - Fetching next pending crawl request");

            // Define the SQL query to execute
            string sql = @"SELECT RequestId, Url
                           FROM CRAWL_REQUEST 
                           WHERE Status = 1 ORDER BY CreatedAt asc LIMIT 1";

            // Create connection and execute the query
            using IDbConnection conn = _dbConnectionManager.CreateConnection();
            IEnumerable<CrawlRequest> taskResult = await conn.QueryAsync<CrawlRequest>(sql);

            if(taskResult == null || taskResult.Count() == 0)
            {
                _logger.LogInformation("CrawlServiceRepository - No pending crawl request to be processed");
                return null;
            }

            _logger.LogInformation("CrawlServiceRepository - Fetched pending crawl request");
            return taskResult.ToList().FirstOrDefault();
        }
    }
}
