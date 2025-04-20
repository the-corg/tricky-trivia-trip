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
        // TODO: remove this (initial debug only)
        private readonly IQuestionQueue _questionQueue;
        private readonly IQuestionRepository _questionRepository;
        private readonly IRepository<AnswerOption> _answerOptionRepository;

        public StatsViewModel(INavigationService navigationService, IQuestionQueue questionQueue, IQuestionRepository questionRepository, IRepository<AnswerOption> answerOptionRepository) : base(navigationService)
        {
            _questionQueue = questionQueue;
            _questionRepository = questionRepository;
            _answerOptionRepository = answerOptionRepository;
            Initialize();
        }

        // TODO: remove this (initial debug only)
        private async void Initialize()
        {
            QuestionQueue q = (QuestionQueue) _questionQueue;
            await q.InitializeAsync();
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

        // TODO: remove this (initial debug only)
        public ObservableCollection<Question> QuestionsFromQueue { get; set; } = new();
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ObservableCollection<AnswerOption> AnswerOptions { get; set; } = new();

    }
}
