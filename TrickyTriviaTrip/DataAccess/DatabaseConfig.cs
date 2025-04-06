using System.IO;

namespace TrickyTriviaTrip.DataAccess
{
    public class DatabaseConfig
    {

        private string _databaseFileName;
        private string _databaseFolderPath;

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
