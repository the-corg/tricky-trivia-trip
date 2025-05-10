using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for Score
    /// </summary>
    public class ScoreRepository : BaseRepository<Score>
    {
        // Ordinal positions of table columns, lazily loaded in MapToEntity
        private int? _ordinalId;
        private int? _ordinalPlayerId;
        private int? _ordinalValue;
        private int? _ordinalTimestamp;

        public ScoreRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "Score";

        #region CRUD operations
        // For the docstrings, see the interface
        public override async Task AddAsync(Score entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Score (PlayerId, Value) VALUES (@PlayerId, @Value)";
            cmd.Parameters.Add(new SQLiteParameter("@PlayerId", entity.PlayerId));
            cmd.Parameters.Add(new SQLiteParameter("@Value", entity.Value));

            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task UpdateAsync(Score entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Score SET PlayerId = @PlayerId, Value = @Value, Timestamp = @Timestamp WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@PlayerId", entity.PlayerId));
            cmd.Parameters.Add(new SQLiteParameter("@Value", entity.Value));
            cmd.Parameters.Add(new SQLiteParameter("@Timestamp", entity.Timestamp));
            cmd.Parameters.Add(new SQLiteParameter("@Id", entity.Id));

            await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        protected override Score MapToEntity(IDataReader reader)
        {
            return new Score
            {
                Id = reader.GetInt64(_ordinalId ??= reader.GetOrdinal("Id")),
                PlayerId = reader.GetInt64(_ordinalPlayerId ??= reader.GetOrdinal("PlayerId")),
                Value = reader.GetInt32(_ordinalValue ??= reader.GetOrdinal("Value")),
                Timestamp = reader.GetDateTime(_ordinalTimestamp ??= reader.GetOrdinal("Timestamp"))
            };
        }

    }
}
