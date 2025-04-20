namespace TrickyTriviaTrip.Api.ApiResponses
{
    /// <summary>
    /// Structure of the API response to a session token request.
    /// Used only for JSON deserialization
    /// </summary>
    internal class TriviaApiTokenResponse
    {
        public int ResponseCode { get; set; }
        public required string ResponseMessage { get; set; }
        public string? Token { get; set; }
    }
}
