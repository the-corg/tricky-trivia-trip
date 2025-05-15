using System.IO;
using TrickyTriviaTrip.Properties;

namespace TrickyTriviaTrip.DataAccess
{
    // For the docstrings, see the interface
    public class DatabaseConfig : IDatabaseConfig
    {
        private readonly string _databaseFileName;
        private readonly string _databaseFolderPath;

        public DatabaseConfig()
        {
            _databaseFileName = Settings.Default.DatabaseFileName;

            // Path to folder AppData\Local\TrickyTriviaTrip
            _databaseFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                Settings.Default.AppName);

            // Ensure folder exists
            Directory.CreateDirectory(_databaseFolderPath);
        }

        public string FullDatabasePath => Path.Combine(_databaseFolderPath, _databaseFileName);

        // Connection string documentation: https://system.data.sqlite.org/home/doc/tip/System.Data.SQLite/SQLiteConnectionStringBuilder.cs
        public string ConnectionString => $"Data Source={FullDatabasePath};Version=3;Foreign Keys=true;Cache Size=-10000;Default Timeout=30;BusyTimeout=15000;Synchronous=Off;";

    }
}
