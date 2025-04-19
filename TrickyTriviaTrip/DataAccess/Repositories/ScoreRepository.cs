using System.Data;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for Score
    /// </summary>
    public class ScoreRepository : BaseRepository<Score>
    {
        public ScoreRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string TableName => "Score";

        #region CRUD operations
        // For the docstrings, see the interface
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
