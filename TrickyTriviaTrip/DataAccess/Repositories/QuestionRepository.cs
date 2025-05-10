using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Services;

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
        /// Inserts Question and all its AnswerOptions into the database in one transaction,
        /// then returns the inserted question and its answers (with all their Ids)
        /// </summary>
        /// <param name="questionWithAnswers">Question to be inserted, together with its answer options</param>
        /// <returns>Either the question with its answer options, or null, if something went wrong</returns>
        Task<QuestionWithAnswers?> InsertWithAnswersAsync(QuestionWithAnswers questionWithAnswers);

        /// <summary>
        /// Returns a collection of questions with answer options that 
        /// either a specific player or all players have answered the least
        /// </summary>
        /// <param name="count">Number of questions to get</param>
        /// <param name="player">A specific player</param>
        /// <returns>Collection of questions with answer options that either a specific player 
        /// or all players (if player is null) have answered the least</returns>
        Task<IEnumerable<QuestionWithAnswers>> GetLeastAnsweredWithAnswersAsync(int count, Player? player = null);

    }


    public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
    {
        // Ordinal positions of table columns, lazily loaded in MapToEntity
        private int? _ordinalId;
        private int? _ordinalText;
        private int? _ordinalDifficulty;
        private int? _ordinalCategory;
        private int? _ordinalContentHash;

        private readonly IAnswerOptionRepository _answerOptionRepository;
        private readonly ILoggingService _loggingService;

        public QuestionRepository(IDbConnectionFactory connectionFactory, IAnswerOptionRepository answerOptionRepository,
            ILoggingService loggingService) : base(connectionFactory)
        {
            _answerOptionRepository = answerOptionRepository;
            _loggingService = loggingService;
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

        // For the docstrings, see the interface
        public async Task<bool> ExistsByHashAsync(string contentHash)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();

            cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM Question WHERE ContentHash = @ContentHash LIMIT 1)";
            cmd.Parameters.Add(new SQLiteParameter("@ContentHash", contentHash));

            var result = await cmd.ExecuteScalarAsync(); // Returns either 0 or 1
            return Convert.ToInt32(result) > 0;
        }

        public async Task<QuestionWithAnswers?> InsertWithAnswersAsync(QuestionWithAnswers questionWithAnswers)
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
                for (int i = 0; i < questionWithAnswers.AnswerOptions.Count; i++)
                {
                    questionWithAnswers.AnswerOptions[i].QuestionId = questionId;

                    using var answerCmd = connection.CreateCommand();
                    answerCmd.CommandText = "INSERT INTO AnswerOption (QuestionId, Text, IsCorrect) VALUES (@QuestionId, @Text, @IsCorrect)";
                    answerCmd.Parameters.Add(new SQLiteParameter("@QuestionId", questionId));
                    answerCmd.Parameters.Add(new SQLiteParameter("@Text", questionWithAnswers.AnswerOptions[i].Text));
                    answerCmd.Parameters.Add(new SQLiteParameter("@IsCorrect", questionWithAnswers.AnswerOptions[i].IsCorrect));

                    await answerCmd.ExecuteNonQueryAsync();

                    // Get and assign id of the inserted answer option
                    var answerId = ((SQLiteConnection)connection).LastInsertRowId;
                    questionWithAnswers.AnswerOptions[i].Id = answerId;
                }

                await transaction.CommitAsync();

                return questionWithAnswers;
            }
            catch (Exception)
            {
                _loggingService.LogError("Error during transaction of inserting a question and its answer options.\nRolling back the transaction...");
                await transaction.RollbackAsync();
                return null;
            }

        }

        public async Task<IEnumerable<QuestionWithAnswers>> GetLeastAnsweredWithAnswersAsync(int count, Player? player = null)
        {
            var resultList = new List<QuestionWithAnswers>();
            var listOfQuestions = new List<Question>();

            // Several parts because one is optional depending on whether player == null
            string commandPart1 = @"SELECT q.*
                                    FROM Question q
                                    LEFT JOIN AnswerAttempt a ON q.Id = a.QuestionId ";
            string commandPart2 = @"AND a.PlayerId = @PlayerId "; // Optional part referring to the player
            string commandPart3 = @"GROUP BY q.Id
                                    ORDER BY COUNT(a.Id) ASC 
                                    LIMIT @Count";

            using var connection = await _connectionFactory.GetConnectionAsync();

            // Command to get the questions
            using var cmd = connection.CreateCommand();
            if (player is not null)
            {
                // Full query for a specific player with an additional parameter
                cmd.CommandText = commandPart1 + commandPart2 + commandPart3;
                cmd.Parameters.Add(new SQLiteParameter("@PlayerId", player.Id));
            }
            else
            {
                // Basic query that counts all players
                cmd.CommandText = commandPart1 + commandPart3;
            }
            // Parameter for both cases
            cmd.Parameters.Add(new SQLiteParameter("@Count", count));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                listOfQuestions.Add(MapToEntity(reader));
            }

            // Get all answer options and pack them together with each question
            foreach (var question in listOfQuestions)
            {
                var answerOptions = await _answerOptionRepository.GetByQuestionIdAsync(question.Id);

                if (answerOptions.Count != 4)
                {
                    // Something happened to the database so that this question doesn't have its 4 answers stored in the DB
                    _loggingService.LogError($"Error getting answer options for question #{question.Id} with text:\n{question.Text}\nExpected 4 answer options, got {answerOptions.Count} instead. Skipping the question.");
                    continue;
                }

                resultList.Add(new QuestionWithAnswers() { Question = question, AnswerOptions = answerOptions });
            }

            return resultList;
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
