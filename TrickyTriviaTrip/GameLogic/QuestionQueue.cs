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
        QuestionWithAnswers GetNextQuestion();

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
        private readonly IQuestionRepository _questionRepository;

        // TODO: Change this to private and to Queue<QuestionWithOptions>
        public readonly Queue<QuestionWithAnswers> _queue = new();
        private bool _isFetching = false;

        public QuestionQueue(ITriviaApiClient triviaApiClient, IQuestionRepository questionRepository)
        {
            _triviaApiClient = triviaApiClient;
            _questionRepository = questionRepository;
        }

        #endregion

        #region Public methods and properties 
        public QuestionWithAnswers GetNextQuestion()
        {
            throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
            await LoadQuestionsAsync(_initialLoadCount);
            // TODO: If too few, fallback to DB
        }

        #endregion


        #region Private helper methods 

        /// <summary>
        /// Loads questions from the API, ignores those already existing in the database
        /// </summary>
        /// <param name="count">Number of questions to load</param>
        /// <returns>The number of actually loaded new questions</returns>
        private async Task<int> LoadQuestionsAsync(int count)
        {
            _isFetching = true;
            int countAdded = 0;

            try
            {
                var apiQuestions = await _triviaApiClient.FetchNewQuestionsAsync(count);

                foreach (var apiQuestion in apiQuestions)
                {
                    // Compute hash of the question and check if it exists in the database
                    var hash = ComputeHash(apiQuestion);
                    bool exists = await _questionRepository.ExistsByHashAsync(hash);
                    // Ignore existing questions
                    if (exists)
                        continue;

                    // Transform the question from API-received object to Question and AnswerOptions
                    var question = new Question()
                    {
                        Text = apiQuestion.Question,
                        Difficulty = apiQuestion.Difficulty,
                        Category = apiQuestion.Category,
                        ContentHash = hash
                    };

                    var questionWithAnswers = new QuestionWithAnswers() { Question = question };

                    questionWithAnswers.AnswerOptions.Add(
                        new AnswerOption() { IsCorrect = true, Text = apiQuestion.CorrectAnswer });

                    questionWithAnswers.AnswerOptions.AddRange(
                        apiQuestion.IncorrectAnswers.Select(
                            x => new AnswerOption() { IsCorrect = false, Text = x }));

                    // Add the new question to the database in one transaction
                    await _questionRepository.InsertWithAnswersAsync(questionWithAnswers);

                    _queue.Enqueue(questionWithAnswers);
                    countAdded++;
                }
            }
            finally
            {
                _isFetching = false;
            }
            return countAdded;
        }


        /// <summary>
        /// Computes SHA256 hash based on the question text + the texts of all of its answer options
        /// </summary>
        /// <param name="apiQuestion">Question, as received from the API</param>
        /// <returns>Hash, as a Base64-encoded string</returns>
        private string ComputeHash(TriviaApiQuestion apiQuestion)
        {
            // Add the question text and all answer options to a list
            List<string> inputList = [apiQuestion.Question, apiQuestion.CorrectAnswer];
            inputList.AddRange(apiQuestion.IncorrectAnswers);

            // Convert the list to a string
            string inputString = string.Join("|", inputList);

            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString)));
        }
        #endregion
    }
}
