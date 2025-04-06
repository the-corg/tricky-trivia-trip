using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class PlayerRepository : BaseRepository<Player>
    {
        public PlayerRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

    }
}
