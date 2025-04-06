using System.Data;
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

        public override Task AddAsync(AnswerOption entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(AnswerOption entity)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override AnswerOption MapToEntity(IDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
