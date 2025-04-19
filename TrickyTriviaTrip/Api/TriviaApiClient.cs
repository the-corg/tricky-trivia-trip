using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.Api
{
    // For the docstrings, see the interface
    public class TriviaApiClient : ITriviaApiClient
    {
        private readonly HttpClient _httpClient;
        private string? _sessionToken;


        public TriviaApiClient(HttpClient httpClient, IRepository<Question> questionRepository, IRepository<AnswerOption> answerOptionRepository)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TriviaApiQuestion>> FetchNewQuestionsAsync(int amount = 10)
        {
            await RequestTokenIfNullAsync();
            
            // TODO: Delete this
            Debug.WriteLine($"Current token: {_sessionToken}");

            var url = Properties.Settings.Default.TriviaApiBaseUrl + 
                "?amount=" + amount +
                "&type=multiple" +
                "&token" + _sessionToken;

            // Ensures correct deserialization of snake_case keys from API response
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            var response = await _httpClient.GetFromJsonAsync<TriviaApiResponse>(url, serializeOptions);

            if (response is null)
                throw new Exception("Failed to get or deserialize Trivia API response");

            // TODO: Handle the response codes

            return response.Results;
        }
        // TODO: catch exceptions


        // Requests a session token from Trivia API (if not requested earlier)
        private async Task RequestTokenIfNullAsync()
        {
            if (_sessionToken is not null)
                return;

            var tokenResponse = await _httpClient.GetFromJsonAsync<TokenResponse>(Properties.Settings.Default.RetrieveTokenUrl);

            if (tokenResponse?.ResponseCode != 0 || string.IsNullOrEmpty(tokenResponse.Token))
                throw new Exception("Failed to get a session token from Trivia API");

            _sessionToken = tokenResponse.Token;
        }



        /// <summary>
        /// Structure of the API response to a session token request.
        /// Used only for JSON deserialization
        /// </summary>
        private class TokenResponse
        {
            public int ResponseCode { get; set; }
            public string ResponseMessage { get; set; }
            public string? Token { get; set; }
        }

        /// <summary>
        /// Structure of the API response to a request for questions.
        /// Used only for JSON deserialization
        /// </summary>
        private class TriviaApiResponse
        {
            public int ResponseCode { get; set; }
            public List<TriviaApiQuestion> Results { get; set; } = new();
        }

        /// <summary>
        /// Structure of one question from the API response to a request for questions.
        /// Used only for JSON deserialization
        /// </summary>
        public class TriviaApiQuestion
        {
            public string Type { get; set; }
            public string Difficulty { get; set; }
            public string Category { get; set; }
            public string Question { get; set; }
            public string CorrectAnswer { get; set; }
            public List<string> IncorrectAnswers { get; set; } = new();
        }


    }
}
