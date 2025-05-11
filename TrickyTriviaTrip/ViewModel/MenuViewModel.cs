using System.ComponentModel;
using System.Windows.Data;
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
        private string _editedPlayerName = "";
        private PlayerViewModel? _selectedPlayer;
        private Operation _currentOperation = Operation.None;
        private bool _isSelectionNotFinished = true;

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
            CancelPlayerEditCommand = new DelegateCommand(execute => CurrentOperation = Operation.None);

            Players = new ListCollectionView(_playData.Players);
            Players.SortDescriptions.Add(new SortDescription("IsCurrent", ListSortDirection.Descending));
            Players.SortDescriptions.Add(new SortDescription("IsDummy", ListSortDirection.Descending));
            
            SelectedPlayer = _playData.CurrentPlayer;
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
        /// Shows the current operation related to the player
        /// </summary>
        private Operation CurrentOperation
        {
            get => _currentOperation;
            set
            {
                _currentOperation = value;
                OnPropertyChanged(nameof(IsNothingBeingDone));
                OnPropertyChanged(nameof(IsEditInProgress));
                OnPropertyChanged(nameof(IsSelectionInProgress));
            }
        }

        /// <summary>
        /// Shows whether the player name is currently being edited
        /// </summary>
        public bool IsEditInProgress => CurrentOperation == Operation.Edit || CurrentOperation == Operation.Add;

        /// <summary>
        /// Shows whether the player is currently being selected
        /// </summary>
        public bool IsSelectionInProgress => CurrentOperation == Operation.Change;

        /* TODO : REMOVE  public bool IsSelectionNotFinished
        {
            get => _isSelectionNotFinished;
            set
            {
                if (_isSelectionNotFinished == value)
                    return;

                _isSelectionNotFinished = value;
                if (!_isSelectionNotFinished)
                    CurrentOperation = Operation.None;
            }

        }*/

        /// <summary>
        /// Shows whether nothing is being done to the player
        /// (neither Edit nor Change)
        /// </summary>
        public bool IsNothingBeingDone => CurrentOperation == Operation.None;

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
        /// All players
        /// </summary>
        public ListCollectionView Players { get; }
        #endregion

        /// <summary>
        /// The player currently selected
        /// </summary>
        public PlayerViewModel? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                if (value is PlayerViewModel p && p.IsDummy)
                {
                    // Add New Player
                    EditedPlayerName = "";
                    _selectedPlayer = _playData.CurrentPlayer;
                    OnPropertyChanged();
                    CurrentOperation = Operation.Add;
                }
                else
                {
                    CurrentOperation = Operation.None;
                    if (value is null)
                        return;

                    _selectedPlayer = value;
                    _playData.CurrentPlayer = _selectedPlayer;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PlayerName));
                }
            }
        }

        #region Commands 

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
            CurrentOperation = Operation.Change;
            _selectedPlayer = _playData.CurrentPlayer;
            OnPropertyChanged(nameof(SelectedPlayer));
            
            Players.Refresh();
        }

        private void EditPlayer()
        {
            if (PlayerName is null)
                return;

            EditedPlayerName = PlayerName;
            CurrentOperation = Operation.Edit;
        }

        private async void ConfirmPlayerEdit()
        {
            if (string.IsNullOrWhiteSpace(EditedPlayerName))
            {
                _loggingService.LogWarning("CanExecute didn't work correctly for ConfirmPlayerEdit. The edited player name is null or empty.");
                return;
            }

            bool isAdd = CurrentOperation == Operation.Add;

            if (isAdd && _playData.Players.Where(x => x.Name == EditedPlayerName).Any())
            {
                _messageService.ShowMessage("Error: A player with this name exists in the database already.\n\nPlease choose a different name");
                _loggingService.LogInfo($"User tried to create a new player named {EditedPlayerName} but a player with this name already existed in the database.");
                return;
            }

            // Stop editing
            CurrentOperation = Operation.None;

            if (isAdd)
            {
                // Create new player and add to the database

                await _playData.AddPlayer(EditedPlayerName);
            }
            else
            {
                // Update an existing player
                if (_playData.CurrentPlayer is null)
                {
                    _loggingService.LogError("Error in ConfirmPlayerEdit: CurrentPlayer is null");
                    return;
                }

                if (_playData.CurrentPlayer.Name == EditedPlayerName)
                    return;

                await _playData.UpdatePlayerName(EditedPlayerName);
            }

            OnPropertyChanged(nameof(PlayerName));
        }

        #endregion

        #region Operation enum

        public enum Operation
        {
            None = 0,
            Change = 1,
            Edit = 2,
            Add = 3
        }

        #endregion

    }

}
