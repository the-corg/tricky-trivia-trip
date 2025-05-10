using System.Collections.ObjectModel;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.GameLogic;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for StatsView
    /// </summary>
    public class StatsViewModel : BaseViewModel
    {
        #region Private fields and the constructor

        // TODO: remove this (initial debug only)
        private readonly IQuestionQueue _questionQueue;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerOptionRepository _answerOptionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IRepository<AnswerAttempt> _anwerAttemptRepository;
        private readonly IRepository<Score> _scoreRepository;


        private readonly INavigationService _navigationService;

        public StatsViewModel(INavigationService navigationService, IQuestionQueue questionQueue, IQuestionRepository questionRepository,
            IAnswerOptionRepository answerOptionRepository, IPlayerRepository playerRepository,
            IRepository<AnswerAttempt> anwerAttemptRepository, IRepository<Score> scoreRepository)
        {
            _navigationService = navigationService;

            BackCommand = new DelegateCommand(execute => _navigationService.NavigateToMenu());

            _questionQueue = questionQueue;
            _questionRepository = questionRepository;
            _answerOptionRepository = answerOptionRepository;
            _playerRepository = playerRepository;
            _scoreRepository = scoreRepository;
            _anwerAttemptRepository = anwerAttemptRepository;
            Initialize();
        }

        #endregion

        // TODO: remove this (initial debug only)
        private async void Initialize()
        {
            QuestionQueue q = (QuestionQueue) _questionQueue;
            var questions = q._queue;
            foreach (var question in questions)
            {
                QuestionsFromQueue.Add(question.Question);
            }

            var dbQuestions = await _questionRepository.GetAllAsync();
            var dbAnswerOptions = await _answerOptionRepository.GetAllAsync();
            var dbPlayers = await _playerRepository.GetAllAsync();
            var dbAnswerAttempts = await _anwerAttemptRepository.GetAllAsync();
            var dbScores = await _scoreRepository.GetAllAsync();
            foreach (var x in dbQuestions)
                Questions.Add(x);
            foreach (var x in dbAnswerOptions)
                AnswerOptions.Add(x);
            foreach (var x in dbPlayers)
                Players.Add(x);
            foreach (var x in dbAnswerAttempts)
                AnswerAttempts.Add(x);
            foreach (var x in dbScores)
                Scores.Add(x);

        }

        #region Public properties

        // TODO: remove this (initial debug only)
        public ObservableCollection<Question> QuestionsFromQueue { get; set; } = new();
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ObservableCollection<AnswerOption> AnswerOptions { get; set; } = new();
        public ObservableCollection<Player> Players { get; set; } = new();
        public ObservableCollection<AnswerAttempt> AnswerAttempts { get; set; } = new();
        public ObservableCollection<Score> Scores { get; set; } = new();


        /// <summary>
        /// Command for the Back to Menu button
        /// </summary>
        public DelegateCommand BackCommand { get; }

        #endregion

    }
}
