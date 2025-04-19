using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for Player
    /// </summary>
    public class PlayerRepository : BaseRepository<Player>
    {
        // Ordinal positions of table columns, lazily loaded in MapToEntity
        private int? _ordinalId;
        private int? _ordinalName;

        public PlayerRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "Player";


        #region CRUD operations
        // For the docstrings, see the interface
        public override async Task AddAsync(Player entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Player (Name) VALUES (@Name)";
            cmd.Parameters.Add(new SQLiteParameter("@Name", entity.Name));

            await cmd.ExecuteNonQueryAsync();
        }

        public override async Task UpdateAsync(Player entity)
        {
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Player SET Name = @Name WHERE Id = @Id";
            cmd.Parameters.Add(new SQLiteParameter("@Name", entity.Name));
            cmd.Parameters.Add(new SQLiteParameter("@Id", entity.Id));

            await cmd.ExecuteNonQueryAsync();
        }
        #endregion

        protected override Player MapToEntity(IDataReader reader)
        {
            return new Player
            {
                Id = reader.GetInt64(_ordinalId ??= reader.GetOrdinal("Id")),
                Name = reader.GetString(_ordinalName ??= reader.GetOrdinal("Name"))
            };
        }

    }
}
