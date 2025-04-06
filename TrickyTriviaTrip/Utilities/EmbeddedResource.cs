using System.IO;
using System.Reflection;

namespace TrickyTriviaTrip.Utilities
{
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
