namespace TrickyTriviaTrip.Api.ApiResponses
{
    /// <summary>
    /// Structure of the API response to a request for questions.
    /// Used only for JSON deserialization
    /// </summary>
    internal class TriviaApiResponse
    {
        public int ResponseCode { get; set; }
        public List<TriviaApiQuestion> Results { get; set; } = new();
    }
}
