using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for AnswerOption
    /// </summary>
    public interface IAnswerOptionRepository : IRepository<AnswerOption>
    {
        /// <summary>
        /// Gets a list of answer options with a particular QuestionId
        /// </summary>
        /// <param name="questionId">QuestionId to look for</param>
        /// <returns>The list of answer options with the provided QuestionId</returns>
        Task<List<AnswerOption>> GetByQuestionIdAsync(long questionId);

    }

    public class AnswerOptionRepository : BaseRepository<AnswerOption>, IAnswerOptionRepository
    {
        // Ordinal positions of table columns, lazily loaded in MapToEntity
        private int? _ordinalId;
        private int? _ordinalQuestionId;
        private int? _ordinalText;
        private int? _ordinalIsCorrect;

        public AnswerOptionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "AnswerOption";


        #region CRUD operations
        // For the docstrings, see the interface
        public override async Task AddAsync(AnswerOption entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO AnswerOption (QuestionId, Text, IsCorrect) VALUES (@QuestionId, @Text, @IsCorrect)";
            cmd.Parameters.Add(new SQLiteParameter("@QuestionId", entity.QuestionId));
            cmd.Parameters.Add(new SQLiteParameter("@Text", entity.Text));
            cmd.Parameters.Add(new SQLiteParameter("@IsCorrect", entity.IsCorrect));

            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task UpdateAsync(AnswerOption entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE AnswerOption SET QuestionId = @QuestionId, Text = @Text, IsCorrect = @IsCorrect WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@QuestionId", entity.QuestionId));
            cmd.Parameters.Add(new SQLiteParameter("@Text", entity.Text));
            cmd.Parameters.Add(new SQLiteParameter("@IsCorrect", entity.IsCorrect));
            cmd.Parameters.Add(new SQLiteParameter("@Id", entity.Id));

            await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        #region Public methods specific to AnswerOption (IAnswerOptionRepository) 

        // For the docstrings, see the interface
        public async Task<List<AnswerOption>> GetByQuestionIdAsync(long questionId)
        {
            var list = new List<AnswerOption>();
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM AnswerOption WHERE QuestionId = @QuestionId";
            cmd.Parameters.Add(new SQLiteParameter("@QuestionId", questionId));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToEntity(reader));
            }

            return list;
        }

        #endregion


        protected override AnswerOption MapToEntity(IDataReader reader)
        {
            return new AnswerOption
            {
                Id = reader.GetInt64(_ordinalId ??= reader.GetOrdinal("Id")),
                QuestionId = reader.GetInt64(_ordinalQuestionId ??= reader.GetOrdinal("QuestionId")),
                Text = reader.GetString(_ordinalText ??= reader.GetOrdinal("Text")),
                IsCorrect = reader.GetBoolean(_ordinalIsCorrect ??= reader.GetOrdinal("IsCorrect"))
            };
        }

    }
}
