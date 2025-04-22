using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for MenuView
    /// </summary>
    public class MenuViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public MenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            StartGameCommand = new DelegateCommand(execute => _navigationService.NavigateToGame());
            ViewStatsCommand = new DelegateCommand(execute => _navigationService.NavigateToStats());
            ExitGameCommand = new DelegateCommand(execute => System.Windows.Application.Current.Shutdown());
        }

        /// <summary>
        /// The name of the current player
        /// </summary>
        public string? PlayerName => Environment.UserName;

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

    }
}
