using System.Data.Common;

namespace TrickyTriviaTrip.DataAccess
{
    public interface IDbConnectionFactory
    {
        // Using DbConnection because IDbConnection doesn't have
        // async methods like OpenAsync, ExecuteNonQueryAsync
        Task<DbConnection> GetConnectionAsync();
    }
}
