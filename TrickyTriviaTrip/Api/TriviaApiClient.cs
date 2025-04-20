using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TrickyTriviaTrip.Api.ApiResponses;
using TrickyTriviaTrip.Properties;

namespace TrickyTriviaTrip.Api
{
    // For the docstrings, see the interface
    public class TriviaApiClient : ITriviaApiClient
    {
        #region Private fields and the constructor 

        private readonly HttpClient _httpClient;
        private string? _sessionToken;

        // Ensures correct deserialization of snake_case keys from API response
        private JsonSerializerOptions _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        public TriviaApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        #endregion

        #region Public methods 

        public async Task<IEnumerable<TriviaApiQuestion>> FetchNewQuestionsAsync(int amount = 10)
        {
            await RequestTokenIfNullAsync();
            // TODO: check that it's not null, fall back to DB
            
            // TODO: Delete this
            Debug.WriteLine($"Current token: {_sessionToken}");

            var url = Settings.Default.TriviaApiBaseUrl + 
                "?amount=" + amount +
                "&type=multiple" +
                "&token" + _sessionToken;

            var response = await _httpClient.GetFromJsonAsync<TriviaApiResponse>(url, _serializerOptions);

            if (response is null)
                // TODO: Change to normal validation without exception (falls back to DB)
                throw new Exception("Failed to get or deserialize Trivia API response");

            // TODO: Handle the response codes

            return response.Results;
        }
        // TODO: catch exceptions, including JSON deserialization exceptions due to required properties
        #endregion

        #region Private helper methods 

        // Requests a session token from Trivia API (if not requested earlier)
        private async Task RequestTokenIfNullAsync()
        {
            if (_sessionToken is not null)
                return;

            var tokenResponse = await _httpClient.GetFromJsonAsync<TriviaApiTokenResponse>(
                Settings.Default.RetrieveTokenUrl, _serializerOptions);

            if (tokenResponse?.ResponseCode != 0 || string.IsNullOrEmpty(tokenResponse.Token))
                // TODO: Change to normal validation without exception (falls back to DB)
                throw new Exception("Failed to get a session token from Trivia API");

            _sessionToken = tokenResponse.Token;
        }
        #endregion
    }
}
