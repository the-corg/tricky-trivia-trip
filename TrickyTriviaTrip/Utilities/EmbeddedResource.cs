using System.IO;
using System.Reflection;

namespace TrickyTriviaTrip.Utilities
{
    /// <summary>
    /// Utility class for reading text from an embedded resource (e.g., .sql script file)
    /// </summary>
    public static class EmbeddedResource
    {
        public static async Task<string> ReadAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}
