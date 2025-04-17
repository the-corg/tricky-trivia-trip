using System.Data.Common;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Creates and opens a database connection
    /// </summary>
    public interface IDbConnectionFactory
    {
        // Using DbConnection because IDbConnection doesn't have
        // async methods like OpenAsync, ExecuteNonQueryAsync
        Task<DbConnection> GetConnectionAsync();
    }
}
