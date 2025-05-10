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
        private bool _isPlayerEditInProgress;
        private bool _isPlayerChangeInProgress;
        private string _editedPlayerName = "";

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
            ChangePlayerCommand = new DelegateCommand(execute => ChangePlayer(), canExecute => _playData.CurrentPlayer is not null);
            EditPlayerCommand = new DelegateCommand(execute => EditPlayer(), canExecute => _playData.CurrentPlayer is not null);
            ConfirmPlayerEditCommand = new DelegateCommand(execute => ConfirmPlayerEdit(), canExecute => !string.IsNullOrWhiteSpace(EditedPlayerName));
            CancelPlayerEditCommand = new DelegateCommand(execute => CancelPlayerEdit());
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
            ChangePlayerCommand.OnCanExecuteChanged();
            EditPlayerCommand.OnCanExecuteChanged();
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
        /// Shows whether the player name is currently edited
        /// </summary>
        public bool IsPlayerEditInProgress
        {
            get => _isPlayerEditInProgress;
            set
            {
                if (_isPlayerEditInProgress == value)
                    return;

                _isPlayerEditInProgress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNothingBeingDoneToPlayer));
            }
        }

        /// <summary>
        /// Shows whether the player is currently being changed
        /// </summary>
        public bool IsPlayerChangeInProgress
        {
            get => _isPlayerChangeInProgress;
            set
            {
                if (_isPlayerChangeInProgress == value)
                    return;

                _isPlayerChangeInProgress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNothingBeingDoneToPlayer));
            }
        }

        /// <summary>
        /// Tentative player name while it's being edited
        /// </summary>
        public string EditedPlayerName
        {
            get => _editedPlayerName;
            set
            {
                if (value == _editedPlayerName)
                    return;

                _editedPlayerName = value;
                OnPropertyChanged();
                ConfirmPlayerEditCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Shows whether nothing is being done to the player
        /// (neither Edit nor Change)
        /// </summary>
        public bool IsNothingBeingDoneToPlayer => !IsPlayerChangeInProgress && !IsPlayerEditInProgress;

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
        /// <summary>
        /// Command for the Change player button
        /// </summary>
        public DelegateCommand ChangePlayerCommand { get; }
        /// <summary>
        /// Command for the Edit player name button
        /// </summary>
        public DelegateCommand EditPlayerCommand { get; }
        /// <summary>
        /// Command for the Confirm player name edit button
        /// </summary>
        public DelegateCommand ConfirmPlayerEditCommand { get; }
        /// <summary>
        /// Command for the Cancel player name edit button
        /// </summary>
        public DelegateCommand CancelPlayerEditCommand { get; }

        #endregion

        #region Private methods

        private void ChangePlayer()
        {
            IsPlayerChangeInProgress = true;
            IsPlayerEditInProgress = false;
        }

        private void EditPlayer()
        {
            if (PlayerName is null)
                return;

            EditedPlayerName = PlayerName;
            IsPlayerEditInProgress = true;
            IsPlayerChangeInProgress = false;
        }

        private async void ConfirmPlayerEdit()
        {
            if (string.IsNullOrWhiteSpace(EditedPlayerName))
            {
                _loggingService.LogWarning("CanExecute didn't work correctly for ConfirmPlayerEdit. The edited player name is null or empty.");
                return;
            }
            
            IsPlayerEditInProgress = false;
            IsPlayerChangeInProgress = false;

            if (_playData.CurrentPlayer is null)
            {
                _loggingService.LogError("Error in ConfirmPlayerEdit: CurrentPlayer is null");
                return;
            }
            
            await _playData.UpdatePlayerName(EditedPlayerName);

            OnPropertyChanged(nameof(PlayerName));
        }

        private void CancelPlayerEdit()
        {
            IsPlayerEditInProgress = false;
            IsPlayerChangeInProgress = false;
        }

        #endregion

    }
}
