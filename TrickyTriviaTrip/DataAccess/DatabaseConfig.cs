using System.IO;

namespace TrickyTriviaTrip.DataAccess
{
    // For the docstrings, see the interface
    public class DatabaseConfig : IDatabaseConfig
    {
        private readonly string _databaseFileName;
        private readonly string _databaseFolderPath;

        public DatabaseConfig()
        {
            _databaseFileName = Properties.Settings.Default.DatabaseFileName;

            // Path to folder AppData\Local\TrickyTriviaTrip
            _databaseFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                Properties.Settings.Default.AppName);

            // Ensure folder exists
            Directory.CreateDirectory(_databaseFolderPath);
        }

        public string FullDatabasePath => Path.Combine(_databaseFolderPath, _databaseFileName);

        public string ConnectionString => $"Data Source={FullDatabasePath};{Properties.Settings.Default.SqliteVersion}";

    }
}
