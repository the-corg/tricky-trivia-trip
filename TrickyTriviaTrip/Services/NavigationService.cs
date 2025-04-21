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

        // Have to initialize this lazily to avoid circular dependency
        private MenuViewModel? _menuViewModel;

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

        public void NavigateToMenu()
        {
            // Always the same MenuViewModel
            CurrentViewModel = _menuViewModel ??= _serviceProvider.GetRequiredService<MenuViewModel>();
        }

        public void NavigateToGame()
        {
            // New GameViewModel every time 
            CurrentViewModel = _serviceProvider.GetRequiredService<GameViewModel>();
        }

        public void NavigateToStats()
        {
            // New StatsViewModel every time 
            CurrentViewModel = _serviceProvider.GetRequiredService<StatsViewModel>();
        }

    }
}
