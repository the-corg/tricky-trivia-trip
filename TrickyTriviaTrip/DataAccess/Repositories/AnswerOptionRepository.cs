using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class AnswerOptionRepository : BaseRepository<AnswerOption>
    {
        public AnswerOptionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

    }
}
