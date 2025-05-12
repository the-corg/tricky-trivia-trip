using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly INavigationService _navigationService;

        private PlayerViewModel? _selectedPlayer;

        public StatsViewModel(IQuestionRepository questionRepository, IAnswerOptionRepository answerOptionRepository,
            IPlayerRepository playerRepository, IAnswerAttemptRepository anwerAttemptRepository,
            IScoreRepository scoreRepository, IPlayData playData, INavigationService navigationService)
        {
            _questionRepository = questionRepository;
            _answerOptionRepository = answerOptionRepository;
            _playerRepository = playerRepository;
            _scoreRepository = scoreRepository;
            _anwerAttemptRepository = anwerAttemptRepository;

            _playData = playData;
            _navigationService = navigationService;

            BackCommand = new DelegateCommand(execute => _navigationService.NavigateToMenu());

            Players = new ListCollectionView(_playData.Players);
            // Filter out the "dummy player"
            Players.Filter = player => (!((PlayerViewModel)player).IsDummy);
            // And sort players by id
            Players.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));

            _selectedPlayer = _playData.CurrentPlayer;

            Initialize();
        }

        #endregion


        #region Data Initialization

        private async void Initialize()
        {




            // TODO: These are for debug tabs. Remove or comment out later
            var dbQuestions = await _questionRepository.GetAllAsync();
            foreach (var x in dbQuestions) Questions.Add(x);

            var dbAnswerOptions = await _answerOptionRepository.GetAllAsync();
            foreach (var x in dbAnswerOptions) AnswerOptions.Add(x);

            var dbPlayers = await _playerRepository.GetAllAsync();
            foreach (var x in dbPlayers) PlayersRaw.Add(x);

            var dbAnswerAttempts = await _anwerAttemptRepository.GetAllAsync();
            foreach (var x in dbAnswerAttempts) AnswerAttempts.Add(x);

            var dbScores = await _scoreRepository.GetAllAsync();
            foreach (var x in dbScores) Scores.Add(x);
        }
        #endregion


        #region Public properties

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
                // TODO: Update data
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

    }
}
