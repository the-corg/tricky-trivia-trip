using TrickyTriviaTrip.GameLogic;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for MenuView
    /// </summary>
    public class MenuViewModel : BaseViewModel
    {
        #region Private fields and the constructor

        private readonly IPlayData _playData;
        private readonly INavigationService _navigationService;
        private readonly ILoggingService _loggingService;
        private readonly IMessageService _messageService;

        public MenuViewModel(IPlayData playData, INavigationService navigationService,
            ILoggingService loggingService, IMessageService messageService)
        {
            _playData = playData;
            _navigationService = navigationService;
            _loggingService = loggingService;
            _messageService = messageService;

            ViewStatsCommand = new DelegateCommand(execute => _navigationService.NavigateToStats());
            ExitGameCommand = new DelegateCommand(execute => App.Current.Shutdown());
            StartGameCommand = new DelegateCommand(execute => _navigationService.NavigateToGame(), canExecute => _playData.CurrentPlayer is not null);
        }
        #endregion


        #region Public methods and properties

        /// <summary>
        /// The name of the current player
        /// </summary>
        public string? PlayerName => _playData.CurrentPlayer?.Name;

        /// <summary>
        /// Updates the player name (for example, after successful initialization)
        /// </summary>
        public void UpdatePlayerName()
        {
            OnPropertyChanged(nameof(PlayerName));
            StartGameCommand.OnCanExecuteChanged();
        }

        /// <summary>
        /// Greeting to the player
        /// </summary>
        public string GreetingText => (_playData.QuestionsAnswered == 0) ? "Welcome," : (_playData.Score == 0) ? "Hi again," : "Good job,";

        /// <summary>
        /// Line about the recent score, if any
        /// </summary>
        public string ScoreText => (_playData.QuestionsAnswered == 0) ? "" : $"Your score is: {_playData.Score}";

        /// <summary>
        /// Line about the number of correct answers, if there was a recent game session
        /// </summary>
        public string CorrectAnswersText => (_playData.QuestionsAnswered == 0) ? "" : $"You answered {_playData.QuestionsAnsweredCorrectly}/{_playData.QuestionsAnswered} questions correctly";

        /// <summary>
        /// Command for the Start Game button
        /// </summary>
        public DelegateCommand StartGameCommand { get; }
        /// <summary>
        /// Command for the View Stats button
        /// </summary>
        public DelegateCommand ViewStatsCommand { get; }
        /// <summary>
        /// Command for the Exit Game button
        /// </summary>
        public DelegateCommand ExitGameCommand { get; }

        #endregion

        #region Private methods


        #endregion

    }
}
