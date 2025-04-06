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

        #endregion

    }
}
