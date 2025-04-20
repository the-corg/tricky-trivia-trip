using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess.Queries
{
    /// <summary>
    /// Manages complex question-related queries
    /// </summary>
    public interface IQuestionQueries
    {
        /// <summary>
        /// Gets a number of questions with answer options from the database asynchronously
        /// </summary>
        /// <param name="count">Number of questions to get</param>
        /// <returns>A collection of questions with their answer options</returns>
        Task<IEnumerable<QuestionWithAnswers>> GetQuestionsWithAnswersAsync(int count);
    }

    public class QuestionQueries : IQuestionQueries
    {
        public Task<IEnumerable<QuestionWithAnswers>> GetQuestionsWithAnswersAsync(int count)
        {
            throw new NotImplementedException();
        }
    }
}
