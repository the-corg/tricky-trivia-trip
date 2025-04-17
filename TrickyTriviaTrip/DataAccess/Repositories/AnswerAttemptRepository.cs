using System.Data;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Provides basic database operations for AnswerAttempt
    /// </summary>
    public class AnswerAttemptRepository : BaseRepository<AnswerAttempt>
    {
        public AnswerAttemptRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string TableName => "AnswerAttempt";


        #region CRUD operations

        public override Task AddAsync(AnswerAttempt entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(AnswerAttempt entity)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override AnswerAttempt MapToEntity(IDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
