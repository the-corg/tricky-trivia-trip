namespace TrickyTriviaTrip.DataAccess
{
    public interface IDatabaseInitializer
    {
        Task InitializeIfMissingAsync();
    }
}
