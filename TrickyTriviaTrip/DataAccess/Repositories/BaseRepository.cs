
namespace TrickyTriviaTrip.DataAccess
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnectionFactory _connectionFactory;

        protected BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public T GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}
