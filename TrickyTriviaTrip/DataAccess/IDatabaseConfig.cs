namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Exposes database connection string and possibly other database configuration parameters
    /// </summary>
    public interface IDatabaseConfig
    {
        /// <summary>
        /// Connection string for the database
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Full path to the file that contains the database
        /// </summary>
        string FullDatabasePath { get; }
    }
}
