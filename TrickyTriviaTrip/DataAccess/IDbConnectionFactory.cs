using System.Data;

namespace TrickyTriviaTrip.DataAccess
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
