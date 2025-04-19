using static TrickyTriviaTrip.Api.TriviaApiClient;

namespace TrickyTriviaTrip.Api
{
    /// <summary>
    /// Trivia API operations
    /// </summary>
    public interface ITriviaApiClient
    {
        /// <summary>
        /// Fetches a batch of questions from Trivia API (multiple choice type, any difficulty, any category)
        /// </summary>
        /// <param name="amount">Number of questions to fetch</param>
        /// <returns>Collection of questions</returns>
        Task<IEnumerable<TriviaApiQuestion>> FetchNewQuestionsAsync(int amount = 10);

    }
}
