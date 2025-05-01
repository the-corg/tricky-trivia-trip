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
        private readonly IRepository<AnswerOption> _answerOptionRepository;


        private readonly INavigationService _navigationService;

        public StatsViewModel(INavigationService navigationService, IQuestionQueue questionQueue, IQuestionRepository questionRepository, IRepository<AnswerOption> answerOptionRepository)
        {
            _navigationService = navigationService;

            BackCommand = new DelegateCommand(execute => _navigationService.NavigateToMenu());

            _questionQueue = questionQueue;
            _questionRepository = questionRepository;
            _answerOptionRepository = answerOptionRepository;
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
            foreach (var dbq in dbQuestions)
            {
                Questions.Add(dbq);
            }
            foreach (var ao in dbAnswerOptions)
            {
                AnswerOptions.Add(ao);
            }

        }

        #region Public properties

        // TODO: remove this (initial debug only)
        public ObservableCollection<Question> QuestionsFromQueue { get; set; } = new();
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ObservableCollection<AnswerOption> AnswerOptions { get; set; } = new();


        /// <summary>
        /// Command for the Back to Menu button
        /// </summary>
        public DelegateCommand BackCommand { get; }

        #endregion

    }
}
