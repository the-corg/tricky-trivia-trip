using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using TrickyTriviaTrip.Api;
using TrickyTriviaTrip.Api.ApiResponses;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Properties;
using TrickyTriviaTrip.Services;

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
        /// Dequeues the next question. If less than
        /// Settings.Default.MinBufferThreshold questions are left afterwards,
        /// launches a task in a separate thread to load more
        /// </summary>
        /// <returns>Next question from the queue</returns>
        Task<QuestionWithAnswers> GetNextQuestionAsync();

        /// <summary>
        /// Loads the initial InitialLoadCount questions into the queue asynchronously
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
        // In case of having to fall back to DB questions, load this number from the DB
        private readonly int _databaseLoadCount = Settings.Default.DatabaseLoadCount;


        private readonly ITriviaApiClient _triviaApiClient;
        private readonly IQuestionRepository _questionRepository;
        private readonly IPlayData _playData;
        private readonly IMessageService _messageService;
        private readonly ILoggingService _loggingService;

        // TODO: Change this to private
        public readonly Queue<QuestionWithAnswers> _queue = new();
        private bool _isFetching = false;

        public QuestionQueue(ITriviaApiClient triviaApiClient, IQuestionRepository questionRepository, IPlayData playData,
            IMessageService messageService, ILoggingService loggingService)
        {
            _triviaApiClient = triviaApiClient;
            _questionRepository = questionRepository;
            _playData = playData;
            _messageService = messageService;
            _loggingService = loggingService;
        }

        #endregion


        #region Public methods and properties 

        public async Task InitializeAsync()
        {
            await LoadQuestionsAsync(_initialLoadCount);
        }

        public async Task<QuestionWithAnswers> GetNextQuestionAsync()
        {
            if (_queue.Count == 0)
            {
                _loggingService.LogWarning("No questions in the queue. Requesting questions from the database...");

                // This happens sometimes when the user clicks Start Game immediately
                // or if either the internet connection or Trivia API is down.

                // Using the database as the faster option
                await UrgentFetchAsync(_databaseLoadCount);

                // Still no questions in the queue
                if (_queue.Count == 0)
                {
                    _loggingService.LogError("Failed to get questions from the database. Last attempt to get at least one question from the API...");

                    // Last drastic attempt to get at least one question from the API

                    // Show message from a background thread to let the API request run
                    // while the user is reading the message 
                    new Thread(() => {
                        _loggingService.LogInfo($"Showing a message to the user from a background thread. Thread: {Environment.CurrentManagedThreadId}");
                        _messageService.ShowMessage("Error! The database contains no questions or is corrupt.\nPlease wait a few seconds while we attempt to get a question from Trivia API...");
                    }).Start();

                    await Task.Delay(5001); // API Rate Limit
                    _loggingService.LogInfo("The API rate limit delay ended. Now asking the API for one quesion...");
                    await LoadQuestionsAsync(1);
                }
            }

            // Get one question from the Queue
            QuestionWithAnswers question;
            try
            {
                question = _queue.Dequeue();
            }
            catch (InvalidOperationException exception)
            {
                _loggingService.LogError("Error dequeueing a question (probably no questions in the queue, will have to exit):\n" + exception.ToString());

                _messageService.ShowMessage("Error! No questions available. Exiting...");
                App.Current.Shutdown();
                throw;
            }

            // Background refill if too few questions left
            if (_queue.Count <= _minBufferThreshold && !_isFetching)
            {
                _ = Task.Run(() => LoadQuestionsAsync(_backgroundLoadCount));
            }

            return question;
        }

        #endregion


        #region Private helper methods 

        /// <summary>
        /// Loads questions from the API, falls back to the database
        /// if too few questions were loaded
        /// </summary>
        /// <param name="count">Number of questions to load</param>
        private async Task LoadQuestionsAsync(int count)
        {
            _loggingService.LogInfo($"{count} new questions requested. Currently {_queue.Count} questions in the queue. Current thread: {Environment.CurrentManagedThreadId}");

            await LoadQuestionsFromApiAsync(count);

            // If too few questions were loaded
            // (for example, Trivia API is down or doesn't have any more questions),
            // load some questions from the database
            if (_queue.Count <= _minBufferThreshold)
            {
                _loggingService.LogWarning($"Not enough questions after API request, will request from the DB. Currently {_queue.Count} questions in the queue. Current thread: {Environment.CurrentManagedThreadId}");
                await UrgentFetchAsync(_databaseLoadCount);
                _loggingService.LogInfo($"After the database query, there are {_queue.Count} questions in the queue.");
            }
        }


        /// <summary>
        /// Loads questions from the API, ignores those already existing in the database
        /// </summary>
        /// <param name="count">Number of questions to load</param>
        private async Task LoadQuestionsFromApiAsync(int count)
        {
            _isFetching = true;
            int discardedCount = 0;

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
                    {
                        discardedCount++;
                        continue;
                    }

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
                    var addedQuestion = await _questionRepository.InsertWithAnswersAsync(questionWithAnswers);

                    if (addedQuestion is not null)
                        _queue.Enqueue(addedQuestion);
                }

                if (_loggingService.ShouldLogInfo)
                {
                    // Unnecessary DB query here. Won't execute in production with Info-level logging off
                    var questionsInDb = await _questionRepository.GetAllAsync();
                    _loggingService.LogInfo($"Obtained {apiQuestions.Count()} questions from the API, {discardedCount} of which were discarded as already existing in the DB.\nThere are {questionsInDb.Count()} questions stored in the database now.");   
                }
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error while checking existence by hash or while adding new questions:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error while loading new questions from the API:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
            finally
            {
                _isFetching = false;
            }
            return;
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

        /// <summary>
        /// Gets a number of questions from the database and adds them to the queue
        /// </summary>
        /// <param name="count">Number of questions to get</param>
        private async Task UrgentFetchAsync(int count)
        {
            try
            {
                var questionsWithAnswers = await _questionRepository.GetLeastAnsweredWithAnswersAsync(count, _playData.CurrentPlayer);

                foreach (var q in questionsWithAnswers)
                    _queue.Enqueue(q);
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error while fetching questions from the database:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error while fetching questions from the database:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }
        #endregion
    }
}
