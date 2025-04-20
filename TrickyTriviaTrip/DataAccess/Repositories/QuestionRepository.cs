using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for Question
    /// </summary>
    public class QuestionRepository : BaseRepository<Question>
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
