using System.Data;
using System.Data.SQLite;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Base repository class for database access
    /// </summary>
    /// <typeparam name="T">Model class</typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;

        protected BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected abstract string TableName { get; }
        protected abstract T MapToEntity(IDataReader reader);

        #region CRUD operations
        // For the docstrings, see the interface
        public abstract Task AddAsync(T entity);
        public abstract Task UpdateAsync(T entity);

        public virtual async Task DeleteAsync(long id)
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

        public virtual async Task<T?> GetByIdAsync(long id)
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
