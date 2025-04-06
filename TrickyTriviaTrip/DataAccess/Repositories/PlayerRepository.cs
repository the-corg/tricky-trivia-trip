using System.Data;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess
{
    public class PlayerRepository : BaseRepository<Player>
    {
        public PlayerRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }

        protected override string TableName => "Player";


        #region CRUD operations
        public override Task AddAsync(Player entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(Player entity)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override Player MapToEntity(IDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
