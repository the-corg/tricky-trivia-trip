using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for GameView
    /// </summary>
    public class GameViewModel : BaseViewModel
    {
        public GameViewModel(INavigationService navigationService) : base(navigationService)
        {
            ExitToMenuCommand = new DelegateCommand(execute => _navigationService.NavigateToMenu());
        }

        /// <summary>
        /// Command for the Exit to Menu button
        /// </summary>
        public DelegateCommand ExitToMenuCommand { get; }
    }
}
