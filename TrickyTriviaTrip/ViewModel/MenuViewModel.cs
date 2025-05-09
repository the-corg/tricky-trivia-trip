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
        private readonly INavigationService _navigationService;
        private readonly IPlayData _playData;

        public MenuViewModel(INavigationService navigationService, IPlayData playData)
        {
            _navigationService = navigationService;
            _playData = playData;

            StartGameCommand = new DelegateCommand(execute => _navigationService.NavigateToGame());
            ViewStatsCommand = new DelegateCommand(execute => _navigationService.NavigateToStats());
            ExitGameCommand = new DelegateCommand(execute => System.Windows.Application.Current.Shutdown());
        }
        #endregion


        #region Public properties

        /// <summary>
        /// The name of the current player
        /// </summary>
        public string? PlayerName => _playData.CurrentPlayer.Name;

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

    }
}
