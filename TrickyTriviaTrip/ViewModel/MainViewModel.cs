using System.Data.Common;
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
        #region Private fields and the constructor

        private readonly INavigationService _navigationService;
        private readonly IDatabaseInitializer _databaseInitializer;
        private readonly IQuestionQueue _questionQueue;
        private readonly IMessageService _messageService;
        private readonly ILoggingService _loggingService;

        public MainViewModel(INavigationService navigationService, IDatabaseInitializer databaseInitializer, 
            IQuestionQueue questionQueue, IMessageService messageService, ILoggingService loggingService)
        { 
            _navigationService = navigationService;
            _databaseInitializer = databaseInitializer;
            _questionQueue = questionQueue;
            _messageService = messageService;
            _loggingService = loggingService;

            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationService.NavigateToMenu();
        }
        #endregion

        #region The async initializer

        /// <summary>
        /// Initializes the database if it doesn't exist.<br/>
        /// Initializes the question queue, loading some questions 
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                await _databaseInitializer.InitializeIfMissingAsync();

                await _questionQueue.InitializeAsync();
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error during initialization:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error during initialization:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }
        #endregion

        #region Public properties

        /// <summary>
        /// The view model of the current view to be shown in MainWindow's ContentControl
        /// </summary>
        public BaseViewModel? CurrentViewModel => _navigationService.CurrentViewModel;

        #endregion


        #region Private methods 

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        #endregion
    }
}
