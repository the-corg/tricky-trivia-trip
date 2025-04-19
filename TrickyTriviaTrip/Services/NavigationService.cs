using Microsoft.Extensions.DependencyInjection;
using TrickyTriviaTrip.ViewModel;

namespace TrickyTriviaTrip.Services
{
    /// <summary>
    /// Manages navigation between the views
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// The view model of the currently active view
        /// </summary>
        BaseViewModel? CurrentViewModel { get; }
        /// <summary>
        /// Subscribe to this event to be notified when CurrentViewModel changes
        /// </summary>
        event Action? CurrentViewModelChanged;
        /// <summary>
        /// Navigates to the main menu
        /// </summary>
        void NavigateToMenu();
        /// <summary>
        /// Navigates to the game view
        /// </summary>
        void NavigateToGame();
        /// <summary>
        /// Navigates to the stats view
        /// </summary>
        void NavigateToStats();
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        // Have to initialize these lazily to avoid circular dependency
        private GameViewModel? _gameViewModel;
        private MenuViewModel? _menuViewModel;
        private StatsViewModel? _statsViewModel;

        private BaseViewModel? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BaseViewModel? CurrentViewModel
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
            CurrentViewModel = _gameViewModel ??= _serviceProvider.GetRequiredService<GameViewModel>();
        }

        public void NavigateToMenu()
        {
            CurrentViewModel = _menuViewModel ??= _serviceProvider.GetRequiredService<MenuViewModel>();
        }

        public void NavigateToStats()
        {
            CurrentViewModel = _statsViewModel ??= _serviceProvider.GetRequiredService<StatsViewModel>();
        }

    }
}
