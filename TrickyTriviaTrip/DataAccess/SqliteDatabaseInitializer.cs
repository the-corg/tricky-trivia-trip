using System.IO;
using TrickyTriviaTrip.Properties;
using TrickyTriviaTrip.Services;
using TrickyTriviaTrip.Utilities;

namespace TrickyTriviaTrip.DataAccess
{
    // For the docstrings, see the interface
    public class SqliteDatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggingService _loggingService;
        private readonly string _databaseFilePath;

        public SqliteDatabaseInitializer(IDbConnectionFactory connectionFactory, IDatabaseConfig dbConfig, ILoggingService loggingService)
        {
            _connectionFactory = connectionFactory;
            _loggingService = loggingService;
            _databaseFilePath = dbConfig.FullDatabasePath;
        }

        public async Task InitializeIfMissingAsync()
        {
            if (File.Exists(_databaseFilePath))
                return;

            _loggingService.LogWarning("Database not found. Creating a new one...");

            // Read database creation script from file (embedded as resource)
            var dbCreationSql = await EmbeddedResource.ReadAsync(Settings.Default.CreateDatabaseScript);

            // Get an open connection (will be automatically closed when disposed of on return)
            using var connection = await _connectionFactory.GetConnectionAsync();

            // Create and run SQL command for database creation
            using var command = connection.CreateCommand();
            command.CommandText = dbCreationSql;
            await command.ExecuteNonQueryAsync();
        }

    }
}
