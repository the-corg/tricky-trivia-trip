using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class ScoreRepository : BaseRepository<Score>
    {
        public ScoreRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

    }
}
