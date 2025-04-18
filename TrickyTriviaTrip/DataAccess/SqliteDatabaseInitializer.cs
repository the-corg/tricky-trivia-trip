using System.IO;
using TrickyTriviaTrip.Utilities;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Creates and initializes the database if it doesn't exist
    /// </summary>
    public class SqliteDatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly string _databaseFilePath;

        public SqliteDatabaseInitializer(IDbConnectionFactory connectionFactory, IDatabaseConfig dbConfig)
        {
            _connectionFactory = connectionFactory;
            _databaseFilePath = dbConfig.FullDatabasePath;
        }

        public async Task InitializeIfMissingAsync()
        {
            if (File.Exists(_databaseFilePath))
                return;

            // Read database creation script from file (embedded as resource)
            var dbCreationSql = await EmbeddedResource.ReadAsync(Properties.Settings.Default.CreateDatabaseScript);

            // Get an open connection (will be automatically closed when disposed of on return)
            using var connection = await _connectionFactory.GetConnectionAsync();

            // Create and run SQL command for database creation
            using var command = connection.CreateCommand();
            command.CommandText = dbCreationSql;
            await command.ExecuteNonQueryAsync();
        }

    }
}
