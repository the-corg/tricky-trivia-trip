using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Windows.Data;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.GameLogic;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for StatsView
    /// </summary>
    public class StatsViewModel : BaseViewModel
    {
        #region Private fields and the constructor

        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerOptionRepository _answerOptionRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IAnswerAttemptRepository _anwerAttemptRepository;
        private readonly IScoreRepository _scoreRepository;
        private readonly IPlayData _playData;
        private readonly IPlayerStatsQueries _playerStatsQueries;
        private readonly INavigationService _navigationService;
        private readonly ILoggingService _loggingService;
        private readonly IMessageService _messageService;

        private PlayerViewModel? _selectedPlayer;

        public StatsViewModel(IQuestionRepository questionRepository, IAnswerOptionRepository answerOptionRepository,
            IPlayerRepository playerRepository, IAnswerAttemptRepository anwerAttemptRepository,
            IScoreRepository scoreRepository, IPlayData playData, IPlayerStatsQueries playerStatsQueries,
            INavigationService navigationService, ILoggingService loggingService, IMessageService messageService)
        {
            _questionRepository = questionRepository;
            _answerOptionRepository = answerOptionRepository;
            _playerRepository = playerRepository;
            _scoreRepository = scoreRepository;
            _anwerAttemptRepository = anwerAttemptRepository;
            _playData = playData;
            _playerStatsQueries = playerStatsQueries;
            _navigationService = navigationService;
            _loggingService = loggingService;
            _messageService = messageService;

            BackCommand = new DelegateCommand(execute => _navigationService.NavigateToMenu());

            Players = new ListCollectionView(_playData.Players);
            // Filter out the "dummy player"
            Players.Filter = player => !((PlayerViewModel)player).IsDummy;
            // And sort players by id
            Players.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));

            HighScores = new ListCollectionView(History);
            // Filter out zero scores
            HighScores.Filter = score => ((ScoreWithPlayerName)score).Value > 0;
            // Sort by score
            HighScores.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Descending));
            // And then by timestamp
            HighScores.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Ascending));

            _selectedPlayer = _playData.CurrentPlayer;

            InitializeData();
        }

        #endregion


        #region Data Initialization 

        private async void InitializeData()
        {
            try
            {
                var scoresWithPlayerNames = await _scoreRepository.GetAllWithPlayerNamesAsync();
                foreach (var score in scoresWithPlayerNames) History.Add(score);

                var averageScores = await _scoreRepository.GetAllAverageAsync();
                foreach (var score in averageScores) AverageScores.Add(score);

                await LoadPlayerStats();


                // TODO: These are for debug tabs. Remove or comment out later
                var questions = await _questionRepository.GetAllAsync();
                foreach (var question in questions) Questions.Add(question);

                var answerOptions = await _answerOptionRepository.GetAllAsync();
                foreach (var answerOption in answerOptions) AnswerOptions.Add(answerOption);

                var players = await _playerRepository.GetAllAsync();
                foreach (var player in players) PlayersRaw.Add(player);

                var answerAttempts = await _anwerAttemptRepository.GetAllAsync();
                foreach (var answerAttempt in answerAttempts) AnswerAttempts.Add(answerAttempt);

                var scores = await _scoreRepository.GetAllAsync();
                foreach (var score in scores) Scores.Add(score);
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error during statistics initialization:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error during statistics initialization:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }

        #endregion


        #region Public properties

        /// <summary>
        /// Scores with player names sorted by value, excluding zero scores
        /// </summary>
        public ListCollectionView HighScores { get; }

        /// <summary>
        /// Scores with player names sorted by timestamp
        /// </summary>
        public ObservableCollection<ScoreWithPlayerName> History { get; set; } = new();

        /// <summary>
        /// Average scores for each player sorted by value
        /// </summary>
        public ObservableCollection<AverageScore> AverageScores { get; set; } = new();

        /// <summary>
        /// Stats for each category for a particular player
        /// </summary>
        public ObservableCollection<AnswerStats> Categories { get; set; } = new();

        /// <summary>
        /// Stats for each difficulty for a particular player
        /// </summary>
        public ObservableCollection<AnswerStats> Difficulties { get; set; } = new();


        /// <summary>
        /// All players
        /// </summary>
        public ListCollectionView Players { get; }

        /// <summary>
        /// The player currently selected
        /// </summary>
        public PlayerViewModel? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                if (value is null)
                    return;

                _selectedPlayer = value;

                OnPropertyChanged();
                UpdatePlayerStats();
            }
        }


        /// <summary>
        /// Command for the Back to Menu button
        /// </summary>
        public DelegateCommand BackCommand { get; }

        #endregion


        #region Debug public properties

        // TODO: These are for debug tabs. Remove or comment out later
        public ObservableCollection<Question> Questions { get; set; } = new();
        public ObservableCollection<AnswerOption> AnswerOptions { get; set; } = new();
        public ObservableCollection<Player> PlayersRaw { get; set; } = new();
        public ObservableCollection<AnswerAttempt> AnswerAttempts { get; set; } = new();
        public ObservableCollection<Score> Scores { get; set; } = new();

        #endregion

        #region Private methods

        /// <summary>
        /// Calculates total values for the summary row for Difficulties and Categories collections
        /// </summary>
        private AnswerStats SummaryRow(List<AnswerStats> collection, Criterion criterion, string criterionText)
        {
            var totalAnswered = collection.Sum(c => c.TotalAnswered);
            var correctlyAnswered = collection.Sum(c => c.CorrectlyAnswered);

            return new AnswerStats()
            {
                Criterion = criterion,
                CriterionText = criterionText,
                TotalAnswered = totalAnswered,
                CorrectlyAnswered = correctlyAnswered,
                CorrectPercentage = 1.0 * correctlyAnswered / totalAnswered
            };
        }

        /// <summary>
        /// Loads data into Categories and Difficulties collections
        /// </summary>
        private async Task LoadPlayerStats()
        {
            if (SelectedPlayer is null)
                return;

            // Load data for categories from the database 
            var categories = await _playerStatsQueries.GetAnswerStatsAsync(SelectedPlayer.Id, Criterion.Category);

            // Add the summary row and then all other rows to the observable collection
            Categories.Add(SummaryRow(categories, Criterion.Category, "All categories (total)"));
            foreach (var category in categories) Categories.Add(category);

            // Load data for difficulties from the database 
            var difficulties = await _playerStatsQueries.GetAnswerStatsAsync(SelectedPlayer.Id, Criterion.Difficulty);

            // Add the summary row and then all other rows to the observable collection
            Difficulties.Add(SummaryRow(difficulties, Criterion.Difficulty, "All difficulties (total)"));
            foreach (var difficulty in difficulties) Difficulties.Add(difficulty);
        }

        /// <summary>
        /// Clears and then reloads data into the Categories and Difficulties collections
        /// </summary>
        private async void UpdatePlayerStats()
        {
            Categories.Clear();
            Difficulties.Clear();

            try
            {
                await LoadPlayerStats();
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error during statistics update:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error during statistics update:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }

            #endregion

        }
    }
}
