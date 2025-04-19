using System.Collections.ObjectModel;
using TrickyTriviaTrip.Api;
using TrickyTriviaTrip.Services;
using static TrickyTriviaTrip.Api.TriviaApiClient;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for StatsView
    /// </summary>
    public class StatsViewModel : BaseViewModel
    {
        // TODO: remove this here and from the constructor (initial debug only)
        private readonly ITriviaApiClient _triviaApiClient;
        public StatsViewModel(INavigationService navigationService, ITriviaApiClient triviaApiClient) : base(navigationService)
        {
            _triviaApiClient = triviaApiClient;
            Initialize();
        }

        // TODO: remove this (initial debug only)
        private async void Initialize()
        {
            var questions = await _triviaApiClient.FetchNewQuestionsAsync();
            foreach (var question in questions)
            {
                TriviaApiQuestions.Add(question);
            }
        }

        // TODO: remove this (initial debug only)
        public ObservableCollection<TriviaApiQuestion> TriviaApiQuestions { get; set; } = new();
    }
}
