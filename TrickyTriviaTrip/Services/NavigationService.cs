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
        #region Private fields and the constructor
        private readonly IServiceProvider _serviceProvider;

        private BaseViewModel? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion

        #region Public properties and events

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

        #endregion


        #region Public navigation methods

        public void NavigateToMenu()
        {
            // New MenuViewModel every time
            CurrentViewModel = _serviceProvider.GetRequiredService<MenuViewModel>();
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
        #endregion

    }
}
