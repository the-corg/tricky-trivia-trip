using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using TrickyTriviaTrip.Api;
using TrickyTriviaTrip.Api.ApiResponses;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Properties;

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
        #region Private fields and the constructor 
        // Load this number of questions at startup 
        private readonly int _initialLoadCount = Settings.Default.InitialLoadCount;
        // Load this number of questions when the threshold is reached
        private readonly int _backgroundLoadCount = Settings.Default.BackgroundLoadCount;
        // If the number of questions reaches this, load more
        private readonly int _minBufferThreshold = Settings.Default.MinBufferThreshold;


        private readonly ITriviaApiClient _triviaApiClient;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<AnswerOption> _answerOptionRepository;

        // TODO: Change this to private and to Queue<QuestionWithOptions>
        public readonly Queue<TriviaApiQuestion> _queue = new();
        private bool _isFetching = false;

        public QuestionQueue(ITriviaApiClient triviaApiClient, IRepository<Question> questionRepository, IRepository<AnswerOption> answerOptionRepository)
        {
            _triviaApiClient = triviaApiClient;
            _questionRepository = questionRepository;
            _answerOptionRepository = answerOptionRepository;
        }

        #endregion

        #region Public methods and properties 
        public QuestionWithOptions GetNextQuestion()
        {
            throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            await LoadQuestionsAsync(_initialLoadCount);
        }

        #endregion


        #region Private helper methods 
        private async Task LoadQuestionsAsync(int count)
        {
            _isFetching = true;

            try
            {
                var apiQuestions = await _triviaApiClient.FetchNewQuestionsAsync(count);
                

                foreach (var q in apiQuestions)
                    _queue.Enqueue(q);
            }
            finally
            {
                _isFetching = false;
            }
        }


        private string ComputeHash(TriviaApiQuestion apiModel)
        {
            List<string> inputList = [apiModel.Difficulty, apiModel.Category, apiModel.Question];
            Debug.WriteLine(inputList.Count);
            inputList.AddRange(apiModel.IncorrectAnswers);
            Debug.WriteLine(inputList.Count);

            string inputString = string.Join("|", inputList);
            Debug.WriteLine(inputString);

            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString)));
        }
        #endregion
    }
}
