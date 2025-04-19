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
            cmd.CommandText = "INSERT INTO Question (Text) VALUES (@Text)";
            cmd.Parameters.Add(new SQLiteParameter("@Text", entity.Text));

            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task UpdateAsync(Question entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Question SET Text = @Text WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@Text", entity.Text));
            cmd.Parameters.Add(new SQLiteParameter("@Id", entity.Id));

            await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        protected override Question MapToEntity(IDataReader reader)
        {
            return new Question
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Text = reader.GetString(reader.GetOrdinal("Text"))
            };
        }

    }
}
