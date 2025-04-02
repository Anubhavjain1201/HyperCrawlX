using Dapper;
using HyperCrawlX.DAL.Interfaces;
using HyperCrawlX.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HyperCrawlX.DAL.Repositories
{
    public class CrawlRequestRepository : ICrawlRequestRepository
    {
        private readonly ILogger<CrawlRequestRepository> _logger;
        private readonly IDbConnectionManager _dbConnectionManager;

        public CrawlRequestRepository(
            ILogger<CrawlRequestRepository> logger,
            IDbConnectionManager dbConnectionManager)
        {
            _logger = logger;
            _dbConnectionManager = dbConnectionManager;
        }

        public async Task<CrawlRequestStatus> GetCrawlRequestStatus(long? requestId)
        {
            _logger.LogInformation("CrawlRequestRepository - Getting crawl request status");

            // Define the SQL query to execute
            string sql = @"SELECT 
                            cr.RequestId, 
                            cr.Status, cr.Url, 
                            NULLIF(COUNT(cu.Id), 0) AS ProductUrlsCount,
                            CASE 
                                WHEN COUNT(cu.Id) = 0 THEN NULL 
                                ELSE ARRAY_AGG(cu.Url) 
                            END AS ProductUrls
                           FROM CRAWL_REQUEST cr
                           LEFT JOIN CRAWLED_URL cu ON cu.RequestId = cr.RequestId
                           WHERE cr.RequestId = @RequestId
                           GROUP BY cr.RequestId";

            // Define the parameters to pass to the query
            DynamicParameters dynamicParams = new DynamicParameters();
            dynamicParams.Add("requestId", requestId, DbType.Int64, ParameterDirection.Input);

            // Create connection and execute the query
            using IDbConnection conn = _dbConnectionManager.CreateConnection();
            IEnumerable<CrawlRequestStatus> taskResult = await conn.QueryAsync<CrawlRequestStatus>(sql, dynamicParams);

            _logger.LogInformation("CrawlRequestRepository - Fetched crawl request status");
            return taskResult.ToList().FirstOrDefault()!;
        }

        public async Task<bool> IsValidRequestId(long requestId)
        {
            _logger.LogInformation($"CrawlRequestRepository - Checking if the requestId: {requestId} is valid");

            // Define the SQL query to execute
            string sql = @"SELECT count(1) FROM CRAWL_REQUEST WHERE RequestId = @requestId";

            // Define the parameters to pass to the query
            DynamicParameters dynamicParams = new DynamicParameters();
            dynamicParams.Add("requestId", requestId, DbType.Int64, ParameterDirection.Input);

            // Create connection and execute the query
            using IDbConnection conn = _dbConnectionManager.CreateConnection();
            int taskResult = await conn.ExecuteScalarAsync<int>(sql, dynamicParams);

            _logger.LogInformation("CrawlRequestRepository - Completed checking requestId");
            return taskResult > 0;
        }

        public async Task<int> SubmitCrawlRequest(string? url)
        {
            _logger.LogInformation($"CrawlRequestRepository - Submitting crawl request for the url: {url}");

            // Define the SQL query to execute
            string sql = @"INSERT INTO CRAWL_REQUEST (Url, Status)
                           VALUES (@url, 1)
                           RETURNING RequestId";

            // Define the parameters to pass to the query
            DynamicParameters dynamicParams = new DynamicParameters();
            dynamicParams.Add("url", url, DbType.String, ParameterDirection.Input);

            // Create connection and execute the query
            using IDbConnection conn = _dbConnectionManager.CreateConnection();
            int taskResult = await conn.ExecuteScalarAsync<int>(sql, dynamicParams);

            _logger.LogInformation("CrawlRequestRepository - Submitted crawl request");
            return taskResult;
        }
    }
}
