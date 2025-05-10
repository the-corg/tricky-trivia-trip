using System.Data.Common;
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

        private readonly Task _playerInitializationTask;

        private bool _isStartGameClicked;

        public MenuViewModel(IPlayData playData, INavigationService navigationService,
            ILoggingService loggingService, IMessageService messageService)
        {
            _playData = playData;
            _navigationService = navigationService;
            _loggingService = loggingService;
            _messageService = messageService;

            _playerInitializationTask = Task.Run(InitializePlayer);

            ViewStatsCommand = new DelegateCommand(execute => _navigationService.NavigateToStats());
            ExitGameCommand = new DelegateCommand(execute => System.Windows.Application.Current.Shutdown());
            StartGameCommand = new DelegateCommand(async execute => 
            { 
                _isStartGameClicked = true;
                StartGameCommand?.OnCanExecuteChanged();
                if (!_playerInitializationTask.IsCompleted) 
                { 
                    _loggingService.LogWarning($"Start Game clicked but Player not yet initialized. Current thread: {Environment.CurrentManagedThreadId}. Waiting...");
                    await _playerInitializationTask;
                } 
                _navigationService.NavigateToGame();
            }, canExecute => !_isStartGameClicked);
        }
        #endregion


        #region Public methods properties

        /// <summary>
        /// The name of the current player
        /// </summary>
        public string? PlayerName => _playData.CurrentPlayer?.Name;

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

        private async Task InitializePlayer()
        {
            try
            {
                await _playData.InitializePlayerAsync();
                OnPropertyChanged(nameof(PlayerName));
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error during player initialization:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error during player initialization:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }
        #endregion

    }
}
