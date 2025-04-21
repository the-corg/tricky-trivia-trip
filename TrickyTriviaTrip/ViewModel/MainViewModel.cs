using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.GameLogic;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for Main Window
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IDatabaseInitializer _databaseInitializer;
        private readonly IQuestionQueue _questionQueue;

        public MainViewModel(INavigationService navigationService, IDatabaseInitializer databaseInitializer, IQuestionQueue questionQueue) : base(navigationService)
        { 
            _databaseInitializer = databaseInitializer;
            _questionQueue = questionQueue;

            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationService.NavigateToMenu();
        }

        /// <summary>
        /// The view model of the current view to be shown in MainWindow's ContentControl
        /// </summary>
        public BaseViewModel? CurrentViewModel => _navigationService.CurrentViewModel;

        /// <summary>
        /// Initializes the database if it doesn't exist.<br/>
        /// Initializes the question queue, loading some questions 
        /// </summary>
        public async Task InitializeAsync()
        {
            await _databaseInitializer.InitializeIfMissingAsync();

            await _questionQueue.InitializeAsync();
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
