﻿using System.Data;

namespace HyperCrawlX.DAL.Interfaces
{
    /// <summary>
    /// Manages database connections
    /// </summary>
    public interface IDbConnectionManager
    {
        /// <summary>
        /// Creates and returns a connection to the database.
        /// </summary>
        IDbConnection CreateConnection();
    }
}
