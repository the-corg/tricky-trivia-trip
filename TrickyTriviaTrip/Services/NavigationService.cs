using TrickyTriviaTrip.ViewModel;

namespace TrickyTriviaTrip.Services
{
    /// <summary>
    /// Manages navigation between the views
    /// </summary>
    public interface INavigationService
    {
        BaseViewModel CurrentViewModel { get; }
        event Action? CurrentViewModelChanged;
        void NavigateToMenu();
        void NavigateToGame();
        void NavigateToStats();
    }

    public class NavigationService : INavigationService
    {
        private readonly GameViewModel _gameViewModel;
        private readonly MenuViewModel _menuViewModel;
        private readonly StatsViewModel _statsViewModel;

        private BaseViewModel _currentViewModel;


        public NavigationService(GameViewModel gameViewModel, MenuViewModel menuViewModel, StatsViewModel statsViewModel)
        { 
            _gameViewModel = gameViewModel;
            _menuViewModel = menuViewModel;
            _statsViewModel = statsViewModel;

            _currentViewModel = _menuViewModel;
        }

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                if (_currentViewModel == value)
                    return;

                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        public event Action? CurrentViewModelChanged;

        public void NavigateToGame()
        {
            CurrentViewModel = _gameViewModel;
        }

        public void NavigateToMenu()
        {
            CurrentViewModel = _menuViewModel;
        }

        public void NavigateToStats()
        {
            CurrentViewModel = _statsViewModel;
        }
    }
}
