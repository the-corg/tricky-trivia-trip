using System.Data.Common;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Services;
using TrickyTriviaTrip.ViewModel;

namespace TrickyTriviaTrip.GameLogic
{
    /// <summary>
    /// Manages logic and data related to answer attempts, scores, and players
    /// </summary>
    public interface IPlayData
    {
        /// <summary>
        /// Initializes the player asynchronously
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// The currently active player
        /// </summary>
        public PlayerViewModel? CurrentPlayer { get; set; }

        /// <summary>
        /// Records an answer attempt in the database
        /// </summary>
        /// <param name="answerOption">Answer option selected by the player</param>
        Task RecordAnswer(AnswerOption answerOption);

        /// <summary>
        /// Records the final score for a game session in the database
        /// </summary>
        Task RecordScore();

        /// <summary>
        /// Updates the name of the current player
        /// </summary>
        /// <param name="newPlayerName">New name for the player</param>
        Task UpdatePlayerName(string newPlayerName);

        /// <summary>
        /// Adds a new player to the database
        /// </summary>
        /// <param name="newPlayerName">Name of the new player</param>
        Task AddPlayer(string newPlayerName);

        /// <summary>
        /// The most recent score
        /// </summary>
        int Score { get; set; }

        /// <summary>
        /// The most recent number of answered questions within one game session
        /// </summary>
        int QuestionsAnswered { get; set; }

        /// <summary>
        /// The most recent number of correctly answered questions within one game session
        /// </summary>
        int QuestionsAnsweredCorrectly { get; set; }

        /// <summary>
        /// A list of all players in the database
        /// </summary>
        List<PlayerViewModel> Players { get; }
    }

    public class PlayData : IPlayData
    {
        #region Private fields, constructor and the initializer

        private readonly IPlayerRepository _playerRepository;
        private readonly IRepository<Score> _scoreRepository;
        private readonly IAnswerAttemptRepository _answerAttemptRepository;
        private readonly ILoggingService _loggingService;
        private readonly IMessageService _messageService;
        
        private PlayerViewModel? _currentPlayer;

        public PlayData(IPlayerRepository playerRepository, IRepository<Score> scoreRepository,
            IAnswerAttemptRepository answerAttemptRepository, ILoggingService loggingService, IMessageService messageService)
        {
            _playerRepository = playerRepository;
            _scoreRepository = scoreRepository;
            _answerAttemptRepository = answerAttemptRepository;
            _loggingService = loggingService;
            _messageService = messageService;
        }

        public async Task InitializeAsync()
        {
            // TODO: delete this
            //await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);

            var currentPlayer = await GetLastActivePlayerAsync();

            var allPlayers = await _playerRepository.GetAllAsync();

            // Dummy item
            Players.Add(new PlayerViewModel(null, true));
            foreach (var player in allPlayers)
                Players.Add(new PlayerViewModel(player));
                
            _currentPlayer = Players.First(x => x.Id == currentPlayer.Id);
            _currentPlayer.IsCurrent = true;
        }
        #endregion


        #region Public properties 

        public List<PlayerViewModel> Players { get; } = [];

        public int Score { get; set; }
        public int QuestionsAnswered { get; set; }
        public int QuestionsAnsweredCorrectly { get; set; }

        public PlayerViewModel? CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (value == _currentPlayer)
                    return;

                _currentPlayer = value;

                // Update IsCurrent for all players
                if (_currentPlayer is not null)
                {
                    foreach (var player in Players)
                        player.IsCurrent = false;
                    _currentPlayer.IsCurrent = true;
                }
            }
        }
        #endregion


        #region Public methods 

        public async Task RecordAnswer(AnswerOption answerOption)
        {
            _loggingService.LogInfo($"Current thread: {Environment.CurrentManagedThreadId}. Recording the answer in the DB: Id: {answerOption.Id}, QuestionId: {answerOption.QuestionId}");

            try
            {
                await _answerAttemptRepository.AddAsync(new AnswerAttempt()
                {
                    PlayerId = CurrentPlayer!.Id,
                    QuestionId = answerOption.QuestionId,
                    AnswerOptionId = answerOption.Id
                });
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error while inserting answer \"" + answerOption.Text + "\"into the database:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error while inserting answer \"" + answerOption.Text + "\"into the database:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }

        public async Task RecordScore()
        {
            _loggingService.LogInfo($"Current thread: {Environment.CurrentManagedThreadId}. Recording the score in the DB: {Score}");

            try
            {
                await _scoreRepository.AddAsync(new Score()
                {
                    Value = Score,
                    PlayerId = CurrentPlayer!.Id
                });
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error while recording score in the database:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error while recording score in the database:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }

        public async Task UpdatePlayerName(string newPlayerName)
        {
            if (CurrentPlayer is null)
                return;

            _loggingService.LogInfo($"Current thread: {Environment.CurrentManagedThreadId}. Player name was edited from {CurrentPlayer.Name} to {newPlayerName}. Updating...");
            CurrentPlayer.Name = newPlayerName;

            try
            {
                await _playerRepository.UpdateAsync(CurrentPlayer.Model!);
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error while updating player name in the database:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error while updating player name in the database:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }

        public async Task AddPlayer(string newPlayerName)
        {
            _loggingService.LogInfo($"Current thread: {Environment.CurrentManagedThreadId}. Adding new player named {newPlayerName} to the database...");
            
            var newPlayer = new Player() { Name = newPlayerName };

            try
            {
                await _playerRepository.AddAsync(newPlayer);
                // Get the added player with the id assigned by the database
                var addedPlayer = await _playerRepository.GetByNameAsync(newPlayerName);

                if (addedPlayer is null)
                {
                    _loggingService.LogError($"Error: Failed to add player {newPlayerName} to the database correctly.");
                }
                else
                {
                    CurrentPlayer = new PlayerViewModel(addedPlayer);
                    Players.Add(CurrentPlayer);
                }
            }
            catch (DbException exception)
            {
                _loggingService.LogError("Database error while adding player to the database:\n" + exception.ToString());
                _messageService.ShowMessage("Database error:\n" + exception.Message);
            }
            catch (Exception exception)
            {
                _loggingService.LogError("Error while adding player to the database:\n" + exception.ToString());
                _messageService.ShowMessage("Error:\n" + exception.Message);
            }
        }
        #endregion


        #region Private methods 

        /// <summary>
        /// Returns the player that made the last answer attempt.<br/>
        /// Failing that, the player with the largest Id (the last one added to the DB).<br/>
        /// Failing that, just creates a new player based on the OS username
        /// </summary>
        /// <returns>The last active player (or a new player)</returns>
        private async Task<Player> GetLastActivePlayerAsync()
        {
            _loggingService.LogInfo("Looking for the last active player...");

            // Find the answer attempt with the largest Id - the last answer attempt added to the database
            var lastAnswerAttempt = await _answerAttemptRepository.GetWithMaxIdAsync();

            if (lastAnswerAttempt is not null)
            {
                var playerWhoAnsweredLast = await _playerRepository.GetByIdAsync(lastAnswerAttempt.PlayerId);
                if (playerWhoAnsweredLast is not null)
                    return playerWhoAnsweredLast;
                else
                    _loggingService.LogError($"The player with id {lastAnswerAttempt.PlayerId}, who made the last answer, does not exist in the database.");
            }

            // Fail #1. No answer attempts in the database (or the player who made the last attempt doesn't exist)
            _loggingService.LogWarning("There are no answer attempts in the database. Looking for the last player added to the database...");

            // Find the player with the largest Id - the last player added to the database
            var lastActivePlayer = await _playerRepository.GetWithMaxIdAsync();

            if (lastActivePlayer is not null)
                return lastActivePlayer;

            // Fail #2. No players in the database
            _loggingService.LogWarning("There are no players in the database. Creating a new player based on the OS username...");

            // Create new player based on the OS username
            var newPlayer = new Player() { Name = Environment.UserName };

            await _playerRepository.AddAsync(newPlayer);
            // Get the added player with the id assigned by the database
            var addedPlayer = await _playerRepository.GetByNameAsync(newPlayer.Name);

            if (addedPlayer is not null)
                return addedPlayer;

            // Fail #3. The newly added player could not be found in the database
            _loggingService.LogError($"Error: Failed to add player {newPlayer.Name} to the database correctly.\nTrying to add player \"NewPlayer\" to see if the problem persists...");

            // Trying again with a simple name
            newPlayer.Name = "NewPlayer";
            await _playerRepository.AddAsync(newPlayer);
            addedPlayer = await _playerRepository.GetByNameAsync(newPlayer.Name);

            if (addedPlayer is not null)
                return addedPlayer;

            // Fail #4. This was the last straw. Log this, inform the user, and rage quit 
            var message = "Fatal Database Error: Failed to add new player to the database. The database is likely to be corrupt. Exiting...";
            _loggingService.LogError(message);
            _messageService.ShowMessage(message);
            App.Current.Shutdown();
            throw new Exception(message);
        }
        #endregion

    }
}
