using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides database operations for Question
    /// </summary>
    public interface IQuestionRepository : IRepository<Question>
    {
        /// <summary>
        /// Checks whether a question already exists in the database by its content hash
        /// </summary>
        /// <param name="contentHash">Hash of the question's content<br/>
        /// (computed when first added by QuestionQueue)</param>
        /// <returns>True, if the question already exists in the database<br/>False, otherwise</returns>
        Task<bool> ExistsByHashAsync(string contentHash);

        /// <summary>
        /// Inserts Question and all its AnswerOptions into the database in one transaction
        /// </summary>
        /// <param name="questionWithAnswers">Question to be inserted, together with its answer options</param>
        Task InsertWithAnswersAsync(QuestionWithAnswers questionWithAnswers);

    }


    public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
    {
        // Ordinal positions of table columns, lazily loaded in MapToEntity
        private int? _ordinalId;
        private int? _ordinalText;
        private int? _ordinalDifficulty;
        private int? _ordinalCategory;
        private int? _ordinalContentHash;

        public QuestionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "Question";


        #region CRUD operations
        // For the docstrings, see the interface
        public override async Task AddAsync(Question entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Question (Text, Difficulty, Category, ContentHash) VALUES (@Text, @Difficulty, @Category, @ContentHash)";
            cmd.Parameters.Add(new SQLiteParameter("@Text", entity.Text));
            cmd.Parameters.Add(new SQLiteParameter("@Difficulty", entity.Difficulty));
            cmd.Parameters.Add(new SQLiteParameter("@Category", entity.Category));
            cmd.Parameters.Add(new SQLiteParameter("@ContentHash", entity.ContentHash));

            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task UpdateAsync(Question entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Question SET Text = @Text, Difficulty = @Difficulty, Category = @Category, ContentHash = @ContentHash WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@Text", entity.Text));
            cmd.Parameters.Add(new SQLiteParameter("@Difficulty", entity.Difficulty));
            cmd.Parameters.Add(new SQLiteParameter("@Category", entity.Category));
            cmd.Parameters.Add(new SQLiteParameter("@ContentHash", entity.ContentHash));
            cmd.Parameters.Add(new SQLiteParameter("@Id", entity.Id));

            await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        #region Public methods specific to Question (IQuestionRepository) 

        public async Task<bool> ExistsByHashAsync(string contentHash)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();

            cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM Question WHERE ContentHash = @ContentHash LIMIT 1)";
            cmd.Parameters.Add(new SQLiteParameter("@ContentHash", contentHash));

            var result = await cmd.ExecuteScalarAsync(); // Returns either 0 or 1
            return Convert.ToInt32(result) > 0;
        }

        public async Task InsertWithAnswersAsync(QuestionWithAnswers questionWithAnswers)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                // First insert the Question
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO Question (Text, Difficulty, Category, ContentHash) VALUES (@Text, @Difficulty, @Category, @ContentHash)";
                cmd.Parameters.Add(new SQLiteParameter("@Text", questionWithAnswers.Question.Text));
                cmd.Parameters.Add(new SQLiteParameter("@Difficulty", questionWithAnswers.Question.Difficulty));
                cmd.Parameters.Add(new SQLiteParameter("@Category", questionWithAnswers.Question.Category));
                cmd.Parameters.Add(new SQLiteParameter("@ContentHash", questionWithAnswers.Question.ContentHash));

                await cmd.ExecuteNonQueryAsync();

                // Get id of the inserted question
                var questionId = ((SQLiteConnection)connection).LastInsertRowId;
                questionWithAnswers.Question.Id = questionId;

                // Insert all answer options
                foreach (var answerOption in questionWithAnswers.AnswerOptions)
                {
                    using var answerCmd = connection.CreateCommand();
                    answerCmd.CommandText = "INSERT INTO AnswerOption (QuestionId, Text, IsCorrect) VALUES (@QuestionId, @Text, @IsCorrect)";
                    answerCmd.Parameters.Add(new SQLiteParameter("@QuestionId", questionId));
                    answerCmd.Parameters.Add(new SQLiteParameter("@Text", answerOption.Text));
                    answerCmd.Parameters.Add(new SQLiteParameter("@IsCorrect", answerOption.IsCorrect));

                    await answerCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                // TODO: Change re-throwing to a message (no need to crash here)
                throw;
            }

        }

        #endregion

        protected override Question MapToEntity(IDataReader reader)
        {
            return new Question
            {
                Id = reader.GetInt64(_ordinalId ??= reader.GetOrdinal("Id")),
                Text = reader.GetString(_ordinalText ??= reader.GetOrdinal("Text")),
                Difficulty = reader.GetString(_ordinalDifficulty ??= reader.GetOrdinal("Difficulty")),
                Category = reader.GetString(_ordinalCategory ??= reader.GetOrdinal("Category")),
                ContentHash = reader.GetString(_ordinalContentHash ??= reader.GetOrdinal("ContentHash"))
            };
        }

    }
}
