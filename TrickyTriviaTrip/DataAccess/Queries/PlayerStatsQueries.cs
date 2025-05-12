using System.Data.SQLite;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// The two types of criteria for which the player stats can be calculated
    /// </summary>
    public enum Criterion
    {
        Category,
        Difficulty
    }


    /// <summary>
    /// Provides complex queries for player stats 
    /// </summary>
    public interface IPlayerStatsQueries
    {
        /// <summary>
        /// Returns the stats for a player for either all categories or all difficulties
        /// depending on the criterion parameter
        /// </summary>
        /// <param name="playerId">The id of the player for which the stats are requested</param>
        /// <param name="criterion">Either category or difficulty</param>
        /// <returns>A list of answer stats, sorted by the percentage of correct answers</returns>
        Task<List<AnswerStats>> GetAnswerStatsAsync(long playerId, Criterion criterion);
    }


    public class PlayerStatsQueries : IPlayerStatsQueries
    {

        #region Private fields and the constructor 

        private readonly IDbConnectionFactory _connectionFactory;
        
        public PlayerStatsQueries(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #endregion


        #region Public methods

        public async Task<List<AnswerStats>> GetAnswerStatsAsync(long playerId, Criterion criterion)
        {
            var list = new List<AnswerStats>();

            // Get either the word "Category" or "Difficulty" as string
            string criterionText = Enum.GetName(typeof(Criterion), criterion)!;

            using var connection = await _connectionFactory.GetConnectionAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $@"SELECT {criterionText}, TotalAnswered, CorrectlyAnswered,
                                        100.0 * CorrectlyAnswered / TotalAnswered AS CorrectPercentage
                                 FROM (SELECT q.{criterionText} AS {criterionText},
                                              COUNT(*) AS TotalAnswered,
                                              SUM(CASE WHEN ao.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectlyAnswered
                                       FROM AnswerAttempt aa
                                       JOIN Question q ON aa.QuestionId = q.Id
                                       JOIN AnswerOption ao ON aa.AnswerOptionId = ao.Id
                                       WHERE aa.PlayerId = @PlayerId
                                       GROUP BY q.{criterionText})
                                 ORDER BY CorrectPercentage DESC";
            cmd.Parameters.Add(new SQLiteParameter("@PlayerId", playerId));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new AnswerStats
                {
                    Criterion = criterion,
                    CriterionText = reader.GetString(0),
                    TotalAnswered = reader.GetInt32(1),
                    CorrectlyAnswered = reader.GetInt32(2),
                    CorrectPercentage = reader.GetDouble(3)
                });
            }

            return list;
        }

        #endregion

    }
}
