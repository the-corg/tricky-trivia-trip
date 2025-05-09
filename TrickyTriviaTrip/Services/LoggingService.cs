using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using TrickyTriviaTrip.Properties;

namespace TrickyTriviaTrip.Services
{
    /// <summary>
    /// Writes a log in a background thread.
    /// Deletes log files older than Settings.Default.DaysToKeepLogs
    /// </summary>
    public interface ILoggingService : IDisposable
    {
        /// <summary>
        /// Determines whether Info-level messages should be logged 
        /// </summary>
        bool ShouldLogInfo { get; set; }

        /// <summary>
        /// Writes an Info-level message into the log
        /// </summary>
        /// <param name="message">Message that should be logged</param>
        void LogInfo(string message);

        /// <summary>
        /// Writes a Warning-level message into the log
        /// </summary>
        /// <param name="message">Message that should be logged</param>
        void LogWarning(string message);

        /// <summary>
        /// Writes an Error-level message into the log
        /// </summary>
        /// <param name="message">Message that should be logged</param>
        void LogError(string message);
    }

    public class LoggingService : ILoggingService
    {
        #region Private fields

        private readonly BlockingCollection<string> _logQueue = new();
           
        private readonly TimeSpan _retentionPeriod = TimeSpan.FromDays(Settings.Default.DaysToKeepLogs);

        private readonly string _logFolderPath;
        private readonly string _currentLogFilePath;

        private readonly Task _writeTask;
        private readonly StreamWriter _writer;

        #endregion

        #region Constructor 

        public LoggingService()
        {
            // Path to folder AppData\Local\TrickyTriviaTrip
            _logFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Settings.Default.AppName, "Logs");
            // Ensure folder exists
            Directory.CreateDirectory(_logFolderPath);

            _currentLogFilePath = Path.Combine(_logFolderPath, $"Log_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");


            _writer = new StreamWriter(_currentLogFilePath, true) { AutoFlush = true };

            // Start background write task
            _writeTask = Task.Run(ProcessLogQueue);
            Log("======= SESSION START =======");

            // Also clean up old logs in the background
            Task.Run(CleanupOldLogs);
        }
        #endregion

        #region Public methods and properties

        public bool ShouldLogInfo { get; set; } = true;

        public void LogInfo(string message) { if (ShouldLogInfo) Log("[i] " + message); }
        public void LogWarning(string message) { Log("[?!] " + message); }
        public void LogError(string message) { Log("[!ERROR!] " + message); }

        public void Dispose()
        {
            Log("======== SESSION END ========\n\n");
            _logQueue.CompleteAdding();
            _writeTask.Wait();
            _writer?.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Private methods

        private void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _logQueue.Add($"\n[{timestamp}] {message}");
        }

        private async Task ProcessLogQueue()
        {
            foreach (var message in _logQueue.GetConsumingEnumerable())
            {
                try
                {
                    await _writer.WriteLineAsync(message);
                }
                catch (Exception exception)
                {
                    {
                        // Log file doesn't work, nowhere else to write,
                        // but it would be unwise to crash because of this
                        Debug.WriteLine($"Error writing log file.\nException: {exception.ToString()}\nOriginal log message: {message}");
                    }
                }
            }
        }

        private void CleanupOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(_logFolderPath, "Log_*.txt");
                foreach (var file in files)
                    if (DateTime.Now - File.GetCreationTime(file) > _retentionPeriod)
                        File.Delete(file);
            }
            catch (Exception exception)
            {
                LogWarning("Error during cleanup of old logs: " + exception.ToString());
            }
        }

        #endregion

    }
}
