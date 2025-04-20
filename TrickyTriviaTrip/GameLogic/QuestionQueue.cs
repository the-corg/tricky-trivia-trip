using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.GameLogic
{
    /// <summary>
    /// Manages the queue of questions.
    /// Automatically loads more when needed.
    /// Synchronizes the API responses with the database
    /// </summary>
    public interface IQuestionQueue
    {
        /// <summary>
        /// Dequeues the next question.
        /// If less than 5 questions are left afterwards,
        /// launches a task in a separate thread to load more
        /// </summary>
        /// <returns>Next question from the queue</returns>
        QuestionWithOptions GetNextQuestion();

        /// <summary>
        /// Loads the initial 10 questions into the queue asynchronously
        /// </summary>
        Task InitializeAsync();
    }

    public class QuestionQueue : IQuestionQueue
    {
        public QuestionWithOptions GetNextQuestion()
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
