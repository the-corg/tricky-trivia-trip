using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class AnswerOptionRepository : BaseRepository<AnswerOption>
    {
        public AnswerOptionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "AnswerOption";


        #region CRUD operations

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

        protected override AnswerOption MapToEntity(IDataReader reader)
        {
            return new AnswerOption
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                QuestionId = reader.GetInt64(reader.GetOrdinal("QuestionId")),
                Text = reader.GetString(reader.GetOrdinal("Text")),
                IsCorrect = reader.GetBoolean(reader.GetOrdinal("IsCorrect"))
            };
        }

    }
}
