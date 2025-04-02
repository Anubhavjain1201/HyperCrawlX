using Dapper;
using HyperCrawlX.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using System.Reflection;

namespace HyperCrawlX.DAL
{
    public class DbContext : IDbContext
    {
        private readonly ILogger<DbContext> _logger;
        private readonly IDbConnectionManager _dbConnectionManager;

        public DbContext(
            ILogger<DbContext> logger,
            IDbConnectionManager dbConnectionManager)
        {
            _logger = logger;
            _dbConnectionManager = dbConnectionManager;
        }

        private NpgsqlCommand CreateCommand(
            NpgsqlConnection connection,
            string commandText,
            object? parameters = null,
            CommandType commandType = CommandType.Text)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.CommandTimeout = 300; // Default command timeout = 5 minutes
            AddCommandParameters(command, parameters);

            return command;
        }

        private void AddCommandParameters(NpgsqlCommand command, object? parameters)
        {
            if (parameters is not null)
            {
                if (parameters is IDictionary<string, object> paramDict)
                {
                    foreach (var kvp in paramDict)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = kvp.Key;
                        parameter.Value = kvp.Value ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }
                }
                else
                {
                    // For anonymous objects or regular classes
                    var properties = parameters.GetType().GetProperties();
                    foreach (var prop in properties ?? Array.Empty<PropertyInfo>())
                    {
                        var value = prop.GetValue(parameters);
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = prop.Name;
                        parameter.Value = value ?? DBNull.Value;
                        command.Parameters.Add(parameter);
                    }
                }
            }
        }

        /*public async Task<dynamic> ExecuteAsync(string sql, object? parameters = null)
        {
            try
            {
                using var connection =  _dbConnectionManager.CreateConnection();
                DynamicParameters dynamicParameters = new();
                dynamicParameters.Add("hierUnitId", 23, DbType.Int64, ParameterDirection.Input);
                connection.ExecuteAsync(sql, parameters);
                using var command = CreateCommand(connection, sql, parameters);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DbContext - Error occurred while executing query: {ex.Message}");
                throw;
            }
        }


        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            try
            {
                using var connection = await _dbConnectionManager.CreateConnection();
                using var command = CreateCommand(connection, sql, parameters);

                var results = new List<T>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(MapData<T>(reader));
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing QueryAsync: {Sql}", sql);
                throw;
            }
        }*/
    }
}
