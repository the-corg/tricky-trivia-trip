using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for AnswerAttempt
    /// </summary>
    public class AnswerAttemptRepository : BaseRepository<AnswerAttempt>
    {
        // Ordinal positions of table columns, lazily loaded in MapToEntity
        private int? _ordinalId;
        private int? _ordinalPlayerId;
        private int? _ordinalQuestionId;
        private int? _ordinalAnswerOptionId;
        private int? _ordinalTimestamp;

        public AnswerAttemptRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "AnswerAttempt";


        #region CRUD operations
        // For the docstrings, see the interface
        public override async Task AddAsync(AnswerAttempt entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO AnswerAttempt (PlayerId, QuestionId, AnswerOptionId) VALUES (@PlayerId, @QuestionId, @AnswerOptionId)";
            cmd.Parameters.Add(new SQLiteParameter("@PlayerId", entity.PlayerId));
            cmd.Parameters.Add(new SQLiteParameter("@QuestionId", entity.QuestionId));
            cmd.Parameters.Add(new SQLiteParameter("@AnswerOptionId", entity.AnswerOptionId));

            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task UpdateAsync(AnswerAttempt entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE AnswerAttempt SET PlayerId = @PlayerId, QuestionId = @QuestionId, AnswerOptionId = @AnswerOptionId, Timestamp = @Timestamp WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@PlayerId", entity.PlayerId));
            cmd.Parameters.Add(new SQLiteParameter("@QuestionId", entity.QuestionId));
            cmd.Parameters.Add(new SQLiteParameter("@AnswerOptionId", entity.AnswerOptionId));
            cmd.Parameters.Add(new SQLiteParameter("@Timestamp", entity.Timestamp));
            cmd.Parameters.Add(new SQLiteParameter("@Id", entity.Id));

            await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        protected override AnswerAttempt MapToEntity(IDataReader reader)
        {
            return new AnswerAttempt
            {
                Id = reader.GetInt64(_ordinalId ??= reader.GetOrdinal("Id")),
                PlayerId = reader.GetInt64(_ordinalPlayerId ??= reader.GetOrdinal("PlayerId")),
                QuestionId = reader.GetInt64(_ordinalQuestionId ??= reader.GetOrdinal("QuestionId")),
                AnswerOptionId = reader.GetInt64(_ordinalAnswerOptionId ??= reader.GetOrdinal("AnswerOptionId")),
                Timestamp = reader.GetDateTime(_ordinalTimestamp ??= reader.GetOrdinal("Timestamp"))
            };
        }

    }
}
