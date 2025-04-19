using System.IO;
using System.Reflection;

namespace TrickyTriviaTrip.Utilities
{
    /// <summary>
    /// Utility class for reading text from an embedded resource (e.g., .sql script file)
    /// </summary>
    public static class EmbeddedResource
    {
        /// <summary>
        /// Reads text from embedded resource asynchronously
        /// </summary>
        /// <param name="resourceName">Full name of the embedded resource (e.g., TrickyTriviaTrip.DatabaseScripts.CreateDatabase.sql)</param>
        /// <returns></returns>
        public static async Task<string> ReadAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}
