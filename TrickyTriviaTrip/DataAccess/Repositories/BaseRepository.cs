using System.Data;
using System.Data.SQLite;

namespace TrickyTriviaTrip.DataAccess
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnectionFactory _connectionFactory;

        protected BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected abstract string TableName { get; }
        protected abstract T MapToEntity(IDataReader reader);

        #region CRUD operations

        public abstract Task AddAsync(T entity);
        public abstract Task UpdateAsync(T entity);

        public virtual async Task Delete(int id)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {TableName} WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@Id", id));

            await cmd.ExecuteNonQueryAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var list = new List<T>();
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {TableName}";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToEntity(reader));
            }

            return list;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {TableName} WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@Id", id));

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapToEntity(reader) : null;
        }
        #endregion

    }
}
