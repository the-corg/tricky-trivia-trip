using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class ScoreRepository : BaseRepository<Score>
    {
        public ScoreRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string TableName => "Score";


        #region CRUD operations

        #endregion

    }
}
