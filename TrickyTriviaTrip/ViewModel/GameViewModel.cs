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

        private readonly Random randomNumberGenerator = new();

        private readonly IQuestionQueue _questionQueue;
        private readonly IPlayData _playData;
        private readonly INavigationService _navigationService;
        private readonly ILoggingService _loggingService;
        private readonly IMessageService _messageService;

        private bool _canCelebrate;

        private Question? _question;
        private AnswerViewModel? _selectedAnswer;

        #endregion

        #region Constructor

        public GameViewModel(IQuestionQueue questionQueue, IPlayData playData,
            INavigationService navigationService, ILoggingService loggingService, IMessageService messageService)
        {
            _questionQueue = questionQueue;
            _playData = playData;
            _navigationService = navigationService;
            _loggingService = loggingService;
            _messageService = messageService;

            _loggingService.LogInfo($"Starting a new game. Player: {_playData.CurrentPlayer?.Name}");

            _playData.Score = 0;
            _playData.QuestionsAnswered = 0;
            _playData.QuestionsAnsweredCorrectly = 0;

            // Load the first question 
            NextQuestion();

            // Setup commands
            NextQuestionCommand = new DelegateCommand(execute => NextQuestion(), canExecute => SelectedAnswer is not null);
            ExitToMenuCommand = new DelegateCommand(async execute =>
            {
                if (CurrentQuestionNumber > GameSessionQuestionsTotal)
                {
                    // Finish the game session normally
                    NextQuestion();
                    return;
                }
                if (_messageService.ShowConfirmation($"You have answered {_playData.QuestionsAnswered} questions out of {GameSessionQuestionsTotal}.\n\nYou could still improve your score of {Score}.\n\nEnd this game early?"))
                {
                    _loggingService.LogInfo($"Current thread: {Environment.CurrentManagedThreadId}. Game session finished per player request after answering {_playData.QuestionsAnswered} questions. Score: {Score}");
                    _navigationService.NavigateToMenu();
                    await _playData.RecordScore();
                }
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
        public Question? Question
        {
            get => _question;
            set
            {
                if (_question == value)
                    return;

                _question = value;
                OnPropertyChanged();
            }
        }

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
                    HandlePlayerAnswer(_selectedAnswer);

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
        public int GameSessionQuestionsTotal => Settings.Default.GameSessionQuestionsTotal;

        /// <summary>
        /// The number of the current question in the game session
        /// </summary>
        public int CurrentQuestionNumber => _playData.QuestionsAnswered + 1;

        /// <summary>
        /// The number of questions answered correctly
        /// </summary>
        public int QuestionsAnsweredCorrectly => _playData.QuestionsAnsweredCorrectly;

        /// <summary>
        /// The current score
        /// </summary>
        public int Score
        {
            get => _playData.Score;
            set
            {
                _playData.Score = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Text of the button that either leads to the next question
        /// or finishes the game session
        /// </summary>
        public string NextQuestionButtonText => (CurrentQuestionNumber < GameSessionQuestionsTotal) ? "⇨ Next Question ⇨" : "Finish Game Session";

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
            if (CurrentQuestionNumber > GameSessionQuestionsTotal)
            {
                // Finish the game session 
                _loggingService.LogInfo($"Current thread: {Environment.CurrentManagedThreadId}. Game session finished after answering {_playData.QuestionsAnswered} questions. Score: {Score}");
                _navigationService.NavigateToMenu();
                return;
            }

            SelectedAnswer = null;
            Question = null;
            Answers.Clear();

            // Get the next question from the queue
            var questionWithAnswers = await _questionQueue.GetNextQuestionAsync();

            // Pack each AnswerOption into AnswerViewModel and then shuffle the list randomly
            var answerOptions = questionWithAnswers.AnswerOptions.
                Select(x => new AnswerViewModel(x)).
                OrderBy(_ => randomNumberGenerator.Next()).ToList();

            // Load the answers into the ObservableCollection
            // (Use dispatcher because if GetNextQuestionAsync didn't succeed synchronously,
            // the current thread could be not the UI thread anymore)
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (var answer in answerOptions)
                    Answers.Add(answer);
            });

            Question = questionWithAnswers.Question;

            OnPropertyChanged(nameof(CurrentQuestionNumber));
            OnPropertyChanged(nameof(NextQuestionButtonText));
        }

        /// <summary>
        /// Handles everything after the player selects an answer
        /// </summary>
        /// <param name="answer">The answer selected by the player</param>
        private async void HandlePlayerAnswer(AnswerViewModel answer)
        {
            answer.IsSelected = true;

            _playData.QuestionsAnswered++;

            if (answer.IsCorrect)
            {
                _playData.QuestionsAnsweredCorrectly++;

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

            await _playData.RecordAnswer(answer.Model);
            if (_playData.QuestionsAnswered >= GameSessionQuestionsTotal)
            {
                // That was the last question in the session, record the score
                await _playData.RecordScore();
            }
        }

        #endregion

    }
}
