using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class AnswerAttemptRepository : BaseRepository<AnswerAttempt>
    {
        public AnswerAttemptRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string TableName => "AnswerAttempt";


        #region CRUD operations

        #endregion

    }
}
