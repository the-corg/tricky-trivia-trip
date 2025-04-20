using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.DataAccess.Queries
{
    /// <summary>
    /// Manages complex question-related queries
    /// </summary>
    public interface IQuestionQueries
    {
        /// <summary>
        /// Gets a question with its answer options from the database asynchronously
        /// </summary>
        /// <returns>A collection of questions with options</returns>
        Task<IEnumerable<QuestionWithOptions>> GetQuestionsWithOptionsAsync();
    }

    public class QuestionQueries : IQuestionQueries
    {
        public Task<IEnumerable<QuestionWithOptions>> GetQuestionsWithOptionsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
