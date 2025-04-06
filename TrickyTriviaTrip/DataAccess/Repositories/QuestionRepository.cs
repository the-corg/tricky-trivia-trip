using System.Data;
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

        public override Task AddAsync(Question entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(Question entity)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override Question MapToEntity(IDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
