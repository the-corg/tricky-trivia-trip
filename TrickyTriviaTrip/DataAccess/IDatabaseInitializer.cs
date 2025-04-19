namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Creates and initializes the database if it doesn't exist
    /// </summary>
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// Checks whether the database already exists.<br/>
        /// If not, creates it and initializes the schema
        /// </summary>
        /// <returns></returns>
        Task InitializeIfMissingAsync();
    }
}
