using System.Collections.ObjectModel;
using TrickyTriviaTrip.GameLogic;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Properties;
using TrickyTriviaTrip.Services;
using TrickyTriviaTrip.Utilities;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for GameView
    /// </summary>
    public class GameViewModel : BaseViewModel
    {
        #region Private fields

        // The number of questions in a game session
        private readonly int _gameSessionQuestionsTotal = Settings.Default.GameSessionQuestionsTotal;

        private readonly Random randomNumberGenerator = new();

        private readonly INavigationService _navigationService;
        private readonly IMessageService _messageService;
        private readonly IQuestionQueue _questionQueue;
        private readonly ILoggingService _loggingService;
        private readonly IPlayData _playData;

        private int _questionsAnsweredTotal = 0;
        private int _questionsAnsweredCorrectly = 0;
        private int _score = 0;
        private bool _canCelebrate;

        private Question? _question;
        private AnswerViewModel? _selectedAnswer;

        #endregion

        #region Constructor

        public GameViewModel(INavigationService navigationService, IMessageService messageService,
            IQuestionQueue questionQueue, ILoggingService loggingService, IPlayData playData)
        {
            _navigationService = navigationService;
            _messageService = messageService;
            _questionQueue = questionQueue;
            _loggingService = loggingService;
            _playData = playData;

            _loggingService.LogInfo($"Starting a new game. Player: {_playData.CurrentPlayer.Name}");

            // Load the first question 
            NextQuestion();

            // Setup commands
            NextQuestionCommand = new DelegateCommand(execute => NextQuestion(), canExecute => SelectedAnswer is not null);
            ExitToMenuCommand = new DelegateCommand(execute =>
            {
                if (_messageService.ShowConfirmation("End this game and return to the menu?"))
                    _navigationService.NavigateToMenu();
            });
            
        }
        #endregion

        #region Commands 

        /// <summary>
        /// Command for the Exit to Menu button
        /// </summary>
        public DelegateCommand ExitToMenuCommand { get; }

        /// <summary>
        /// Command for the Next Question button
        /// </summary>
        public DelegateCommand NextQuestionCommand { get; }

        #endregion

        #region Public properties 

        /// <summary>
        /// The question
        /// </summary>
        public Question? Question => _question;

        /// <summary>
        /// All answer options for the current question
        /// </summary>
        public ObservableCollection<AnswerViewModel> Answers { get; } = new();

        /// <summary>
        /// The answer selected by the user
        /// </summary>
        public AnswerViewModel? SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer == value)
                    return;

                _selectedAnswer = value;

                if (_selectedAnswer is not null)
                {
                    // Case when this answer was selected by the user
                    _selectedAnswer.IsSelected = true;

                    _questionsAnsweredTotal++;

                    if (_selectedAnswer.IsCorrect)
                    {
                        _questionsAnsweredCorrectly++;

                        if (Question?.Difficulty == "Easy")
                            Score += 5;
                        else if (Question?.Difficulty == "Medium")
                            Score += 10;
                        else
                            Score += 15;

                        OnPropertyChanged(nameof(SuccessEmojiLine));
                        OnPropertyChanged(nameof(SuccessTextLine));

                        // The setter fires PropertyChanged, and the animation starts
                        CanCelebrate = true;

                        // That's it. Enough celebrating
                        CanCelebrate = false; 
                    }
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNoAnswerSelected));
                NextQuestionCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Shows whether the user has not selected an answer already
        /// </summary>
        public bool IsNoAnswerSelected => SelectedAnswer is null;

        /// <summary>
        /// The number of questions in a game session
        /// </summary>
        public int GameSessionQuestionsTotal => _gameSessionQuestionsTotal;

        /// <summary>
        /// The number of the current question in the game session
        /// </summary>
        public int CurrentQuestionNumber => _questionsAnsweredTotal + 1;

        /// <summary>
        /// The number of questions answered correctly
        /// </summary>
        public int QuestionsAnsweredCorrectly => _questionsAnsweredCorrectly;

        /// <summary>
        /// The current score
        /// </summary>
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Shows whether celebration of success would be appropriate at the moment
        /// </summary>
        public bool CanCelebrate
        {
            get => _canCelebrate;
            set
            {
                if (_canCelebrate == value)
                    return;

                _canCelebrate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Provides a string of success emojis
        /// </summary>
        public string SuccessEmojiLine => SuccessMessages.Emoji;

        /// <summary>
        /// Provides a success message
        /// </summary>
        public string SuccessTextLine => SuccessMessages.Text;

        #endregion

        #region Private helper methods

        /// <summary>
        /// Loads the next question from the queue
        /// </summary>
        private async void NextQuestion()
        {
            // Get the next question from the queue
            var questionWithAnswers = await _questionQueue.GetNextQuestionAsync();
            _question = questionWithAnswers.Question;

            // Pack each AnswerOption into AnswerViewModel and then shuffle the list randomly
            var answerOptions = questionWithAnswers.AnswerOptions.
                Select(x => new AnswerViewModel(x)).
                OrderBy(_ => randomNumberGenerator.Next()).ToList();

            // Load the answers into the ObservableCollection
            Answers.Clear();
            foreach (var answer in answerOptions)
                Answers.Add(answer);

            // Reset the selected answer and send property changed events for all relevant properties
            SelectedAnswer = null;
            OnPropertyChanged(nameof(Question));
            OnPropertyChanged(nameof(CurrentQuestionNumber));
        }

        #endregion

    }
}
