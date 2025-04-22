using TrickyTriviaTrip.GameLogic;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Properties;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for GameView
    /// </summary>
    public class GameViewModel : BaseViewModel
    {
        // The number of questions in a game session
        private readonly int _gameSessionQuestionsTotal = Settings.Default.GameSessionQuestionsTotal;

        private readonly Random randomNumberGenerator = new();

        private readonly INavigationService _navigationService;
        private readonly IMessageService _messageService;
        private readonly IQuestionQueue _questionQueue;

        private int _questionsAnsweredTotal = 0;
        private int _questionsAnsweredCorrectly = 0;
        private int _score = 0;

        private Question? _question;
        private List<AnswerViewModel> _answerOptions = new();
        private AnswerViewModel? _selectedAnswer;

        public GameViewModel(INavigationService navigationService, IMessageService messageService, IQuestionQueue questionQueue)
        {
            _navigationService = navigationService;
            _messageService = messageService;
            _questionQueue = questionQueue;

            // Load the first question 
            NextQuestion();

            // Setup commands
            NextQuestionCommand = new DelegateCommand(execute => NextQuestion(), canExecute => SelectedAnswer is not null);
            ExitToMenuCommand = new DelegateCommand(execute =>
            {
                if (_messageService.ShowConfirmation("End this game and return to the menu?"))
                    _navigationService.NavigateToMenu();
            });
            Answer1Command = new DelegateCommand(
                execute => SelectedAnswer = _answerOptions.ElementAtOrDefault(0), canExecute => SelectedAnswer is null);
            Answer2Command = new DelegateCommand(
                execute => SelectedAnswer = _answerOptions.ElementAtOrDefault(1), canExecute => SelectedAnswer is null);
            Answer3Command = new DelegateCommand(
                execute => SelectedAnswer = _answerOptions.ElementAtOrDefault(2), canExecute => SelectedAnswer is null);
            Answer4Command = new DelegateCommand(
                execute => SelectedAnswer = _answerOptions.ElementAtOrDefault(3), canExecute => SelectedAnswer is null);
        }

        #region Commands 

        /// <summary>
        /// Command for the Exit to Menu button
        /// </summary>
        public DelegateCommand ExitToMenuCommand { get; }

        /// <summary>
        /// Command for the Next Question button
        /// </summary>
        public DelegateCommand NextQuestionCommand { get; }

        /// <summary>
        /// Command for the Answer1 button
        /// </summary>
        public DelegateCommand Answer1Command { get; }
        /// <summary>
        /// Command for the Answer2 button
        /// </summary>
        public DelegateCommand Answer2Command { get; }
        /// <summary>
        /// Command for the Answer3 button
        /// </summary>
        public DelegateCommand Answer3Command { get; }
        /// <summary>
        /// Command for the Answer4 button
        /// </summary>
        public DelegateCommand Answer4Command { get; }

        #endregion

        #region Public properties 

        /// <summary>
        /// The question
        /// </summary>
        public Question? Question => _question;

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

                        _selectedAnswer!.Text = "✔️⇨ " + _selectedAnswer.Text + " ⇦✔️";
                    }
                    else
                    {
                        _selectedAnswer!.Text = "❌⇨ " + _selectedAnswer.Text + " ⇦❌";
                    }
                }

                OnPropertyChanged();
                Answer1Command.OnCanExecuteChanged();
                Answer2Command.OnCanExecuteChanged();
                Answer3Command.OnCanExecuteChanged();
                Answer4Command.OnCanExecuteChanged();
                NextQuestionCommand.OnCanExecuteChanged();

            }
        }

        /// <summary>
        /// The first answer option
        /// </summary>
        public AnswerViewModel? AnswerOption1 => _answerOptions.ElementAtOrDefault(0);

        /// <summary>
        /// The second answer option
        /// </summary>
        public AnswerViewModel? AnswerOption2 => _answerOptions.ElementAtOrDefault(1);

        /// <summary>
        /// The third answer option
        /// </summary>
        public AnswerViewModel? AnswerOption3 => _answerOptions.ElementAtOrDefault(2);

        /// <summary>
        /// The fourth answer option
        /// </summary>
        public AnswerViewModel? AnswerOption4 => _answerOptions.ElementAtOrDefault(3);

        /// <summary>
        /// Shows whether the user has selected an answer already (for binding to the view)
        /// </summary>
        public bool IsAnswerSelected => SelectedAnswer is not null;

        /// <summary>
        /// The number of questions in a game session (for binding to the view)
        /// </summary>
        public int GameSessionQuestionsTotal => _gameSessionQuestionsTotal;

        /// <summary>
        /// The number of the current question in the game session (for binding to the view)
        /// </summary>
        public int CurrentQuestionNumber => _questionsAnsweredTotal + 1;

        /// <summary>
        /// The number of questions answered correctly (for binding to the view)
        /// </summary>
        public int QuestionsAnsweredCorrectly => _questionsAnsweredCorrectly;

        /// <summary>
        /// The current score (for binding to the view)
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

        #endregion

        #region Private helper methods

        /// <summary>
        /// Loads the next question from the queue
        /// </summary>
        private void NextQuestion()
        {
            // Get the next question from the queue
            var questionWithAnswers = _questionQueue.GetNextQuestion();
            _question = questionWithAnswers.Question;

            // Pack each AnswerOption into ObservableAnswerOption and then shuffle the list randomly
            _answerOptions = questionWithAnswers.AnswerOptions.
                Select(x => new AnswerViewModel(x)).
                OrderBy(_ => randomNumberGenerator.Next()).ToList();

            // Reset the selected answer and send property changed events for all relevant properties
            SelectedAnswer = null;
            OnPropertyChanged(nameof(Question));
            OnPropertyChanged(nameof(AnswerOption1));
            OnPropertyChanged(nameof(AnswerOption2));
            OnPropertyChanged(nameof(AnswerOption3));
            OnPropertyChanged(nameof(AnswerOption4));
            OnPropertyChanged(nameof(CurrentQuestionNumber));
        }

        #endregion

    }
}
