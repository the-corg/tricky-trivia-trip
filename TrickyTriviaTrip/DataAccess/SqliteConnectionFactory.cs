using System.Data;
using Microsoft.Data.Sqlite;

namespace TrickyTriviaTrip.DataAccess
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(DatabaseConfig databaseConfig)
        {
            _connectionString = databaseConfig.ConnectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

    }
}
