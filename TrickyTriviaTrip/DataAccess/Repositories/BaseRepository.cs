namespace TrickyTriviaTrip.DataAccess
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnectionFactory _connectionFactory;
        protected BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

    }
}
