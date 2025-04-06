using System.Data;
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

        public override Task AddAsync(Score entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(Score entity)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override Score MapToEntity(IDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
