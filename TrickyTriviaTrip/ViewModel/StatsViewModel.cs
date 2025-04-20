using System.Collections.ObjectModel;
using TrickyTriviaTrip.Api.ApiResponses;
using TrickyTriviaTrip.GameLogic;
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
        public StatsViewModel(INavigationService navigationService, IQuestionQueue questionQueue) : base(navigationService)
        {
            _questionQueue = questionQueue;
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
                TriviaApiQuestions.Add(question);
            }
        }

        // TODO: remove this (initial debug only)
        public ObservableCollection<TriviaApiQuestion> TriviaApiQuestions { get; set; } = new();
    }
}
