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
            TriviaApiResponse? response = null;

            try
            {
                await RequestTokenIfNullAsync();
                if (_sessionToken is null)
                {
                    Debug.WriteLine("Failed to get a session token from Trivia API");
                    return Enumerable.Empty<TriviaApiQuestion>();
                }

                Debug.WriteLine($"Current token: {_sessionToken}");

                var url = Settings.Default.TriviaApiBaseUrl +
                    "?amount=" + amount +
                    "&type=multiple" +
                    "&token" + _sessionToken;

                response = await _httpClient.GetFromJsonAsync<TriviaApiResponse>(url, _serializerOptions);

                if (response is null)
                {
                    Debug.WriteLine("Failed to get or deserialize Trivia API response");
                    return Enumerable.Empty<TriviaApiQuestion>();
                }

                if (response.ResponseCode != 0)
                {
                    await HandleResponseCode(response.ResponseCode);

                    return Enumerable.Empty<TriviaApiQuestion>();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Trivia API error. Response code: {response?.ResponseCode}. Exception:\n"+ exception.ToString());
                // Something went wrong with the API request, but that's not a big problem
                return Enumerable.Empty<TriviaApiQuestion>();
            }

            return response.Results;
        }

        #endregion

        #region Private helper methods 

        /// <summary>
        /// Requests a session token from Trivia API (if not requested earlier)
        /// </summary>
        private async Task RequestTokenIfNullAsync()
        {
            if (_sessionToken is not null)
                return;

            var tokenResponse = await _httpClient.GetFromJsonAsync<TriviaApiTokenResponse>(
                Settings.Default.RetrieveTokenUrl, _serializerOptions);

            if (tokenResponse?.ResponseCode != 0 || string.IsNullOrEmpty(tokenResponse.Token))
                return;

            _sessionToken = tokenResponse.Token;
        }

        /// <summary>
        /// Handles Trivia API response codes.
        /// If necessary, refreshes the session token
        /// </summary>
        /// <param name="responseCode">Trivia API response code</param>
        private async Task HandleResponseCode(int responseCode)
        {
            string reason;
            switch (responseCode)
            {
                case 1:
                    reason = "No Results.\nCould not return results. The API doesn't have enough questions for your query. (Ex. Asking for 50 Questions in a Category that only has 20.)";
                    break;
                case 2:
                    reason = "Invalid Parameter.\nContains an invalid parameter. Arguments passed in aren't valid. (Ex. Amount = Five)";
                    break;
                case 3:
                    reason = "Token Not Found.\nSession Token does not exist.";
                    break;
                case 4:
                    reason = "Token Empty.\nSession Token has returned all possible questions for the specified query. Resetting the Token is necessary.";
                    break;
                case 5:
                    reason = "Rate Limit.\nToo many requests have occurred. Each IP can only access the API once every 5 seconds.";
                    break;
                default:
                    reason = "Undocumented API error";
                    break;
            }

            Debug.WriteLine($"Unsuccessful API request. Code {responseCode}: " + reason);

            if (responseCode == 3 || responseCode == 4)
            {
                // Refreshing the token
                _sessionToken = null;
                await RequestTokenIfNullAsync();
            }
        }
        #endregion
    }
}
