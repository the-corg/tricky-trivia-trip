using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for Main Window
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel(INavigationService navigationService) : base(navigationService)
        { 
            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationService.NavigateToMenu();
        }

        /// <summary>
        /// The view model of the current view to be shown in MainWindow's ContentControl
        /// </summary>
        public BaseViewModel? CurrentViewModel => _navigationService.CurrentViewModel;

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
