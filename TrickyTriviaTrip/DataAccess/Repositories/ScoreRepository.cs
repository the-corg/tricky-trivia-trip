using System.Data;
using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for Score
    /// </summary>
    public interface IScoreRepository : IRepository<Score>
    {
        /// <summary>
        /// Gets all scores with player names
        /// </summary>
        /// <returns>A collection of all scores with player names</returns>
        Task<List<ScoreWithPlayerName>> GetAllWithPlayerNamesAsync();

        /// <summary>
        /// Gets average scores for each player
        /// </summary>
        /// <returns>A collection with the average score for each player</returns>
        Task<List<AverageScore>> GetAllAverageAsync();
    }


    public class ScoreRepository : BaseRepository<Score>, IScoreRepository
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


        #region Public methods specific to Score (IScoreRepository) 

        public async Task<List<ScoreWithPlayerName>> GetAllWithPlayerNamesAsync()
        {
            var list = new List<ScoreWithPlayerName>();
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT s.Timestamp, p.Name, s.Value 
                                FROM Score s
                                JOIN Player p ON s.PlayerId = p.Id
                                ORDER BY s.Timestamp DESC";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new ScoreWithPlayerName
                {
                    Timestamp = reader.GetDateTime(0),
                    PlayerName = reader.GetString(1),
                    Value = reader.GetInt32(2)
                });
            }

            return list;
        }

        public async Task<List<AverageScore>> GetAllAverageAsync()
        {
            var list = new List<AverageScore>();
            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT p.Name, AVG(s.Value) AS AvgScore, COUNT(s.Id) AS GamesCount, MIN(s.Timestamp), MAX(s.Timestamp) 
                                FROM Score s 
                                JOIN Player p ON s.PlayerId = p.Id 
                                GROUP BY p.Id 
                                ORDER BY AvgScore DESC, GamesCount DESC";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new AverageScore
                {
                    PlayerName = reader.GetString(0),
                    Value = reader.GetDouble(1),
                    NumberOfGames = reader.GetInt32(2),
                    From = reader.GetDateTime(3),
                    To = reader.GetDateTime(4)
                });
            }

            return list;
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
