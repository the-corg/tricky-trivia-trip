namespace TrickyTriviaTrip.DataAccess
{
    /// <summary>
    /// Exposes database connection string and possibly other database configuration parameters
    /// </summary>
    interface IDatabaseConfig
    {
        string ConnectionString { get; }
        string FullDatabasePath { get; }
    }
}
