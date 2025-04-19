using System.Data.Common;
using System.Data.SQLite;

namespace TrickyTriviaTrip.DataAccess
{
    // For the docstrings, see the interface
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(IDatabaseConfig databaseConfig)
        {
            _connectionString = databaseConfig.ConnectionString;
        }

        public async Task<DbConnection> GetConnectionAsync()
        {
            DbConnection connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

    }
}
