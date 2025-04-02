using HyperCrawlX.DAL.Constants;
using HyperCrawlX.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace HyperCrawlX.DAL
{
    /// <summary>
    /// Manages database connections
    /// </summary>
    public class DbConnectionManager : IDbConnectionManager
    {
        private readonly ILogger<DbConnectionManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly NpgsqlDataSource _dataSource;
        private readonly string _connectionString;

        public DbConnectionManager(
            ILogger<DbConnectionManager> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = GetConnectionString();

            _logger.LogInformation("DbConnectionManager - Establishing connection to database");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
            dataSourceBuilder.EnableParameterLogging();
            _dataSource = dataSourceBuilder.Build();
            _logger.LogInformation("DbConnectionManager - Connection to database established");
        }

        /// <summary>
        /// Creates and returns a connection to the database.
        /// </summary>
        public IDbConnection CreateConnection()
        {
            try
            {
                _logger.LogInformation("DbConnectionManager - Creating db connection");
                var dbConnection = _dataSource.OpenConnection();
                _logger.LogInformation("DbConnectionManager - Db connection created");
                return dbConnection;
            }
            catch (Exception ex)
            {
                _logger.LogError($"DbConnectionManager - Exception occurred while creating db connection: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Method to create connection string
        /// </summary>
        private string GetConnectionString()
        {
            _logger.LogInformation("DbConnectionManager - Creating connection string");

            // Get the Db credentials from environment variables or appsettings.json
            var host = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_HOST) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_HOST);
            var port = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_PORT) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_PORT);
            var db = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_DB) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_DB);
            var user = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_USER) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_USER);
            var password = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_PASSWORD) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_PASSWORD);
            var minPoolSize = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_MIN_POOL_SIZE) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_MIN_POOL_SIZE);
            var maxPoolSize = Environment.GetEnvironmentVariable(DbConstants.POSTGRES_MAX_POOL_SIZE) ?? _configuration.GetValue<string>(DbConstants.POSTGRES_MAX_POOL_SIZE);

            string connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password};" +
                                $"Pooling=true;Minimum Pool Size={minPoolSize};Maximum Pool Size={maxPoolSize};" +
                                $"Connection Idle Lifetime=300;Connection Pruning Interval=60;" +
                                $"SSL Mode=Prefer;Trust Server Certificate=true;";

            // TODO: Remove connection string from logs
            _logger.LogInformation($"DbConnectionManager - Connection string created: {connectionString}");
            return connectionString;
        }
    }
}
