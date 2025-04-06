using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class QuestionRepository : BaseRepository<Question>
    {
        public QuestionRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string TableName => "Question";


        #region CRUD operations

        #endregion

    }
}
