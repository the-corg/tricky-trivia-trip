namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Creates and initializes the database if it doesn't exist
    /// </summary>
    public interface IDatabaseInitializer
    {
        Task InitializeIfMissingAsync();
    }
}
