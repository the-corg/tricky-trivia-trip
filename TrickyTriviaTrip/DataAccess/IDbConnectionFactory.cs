using System.Data.Common;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Creates and opens a database connection
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates and opens a database connection asynchronously
        /// </summary>
        /// <returns>Using DbConnection instead of IDbConnection here because the latter<br/>
        /// doesn't provide async methods like OpenAsync, ExecuteNonQueryAsync, etc.</returns>
        Task<DbConnection> GetConnectionAsync();
    }
}
