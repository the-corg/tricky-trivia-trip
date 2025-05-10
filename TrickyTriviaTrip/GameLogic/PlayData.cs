using System.Windows;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.GameLogic
{
    /// <summary>
    /// Manages logic and data related to answer attempts, scores, and players
    /// </summary>
    public interface IPlayData
    {
        /// <summary>
        /// The currently active player
        /// </summary>
        public Player? CurrentPlayer { get; set; }

        /// <summary>
        /// Initializes the player asynchronously
        /// </summary>
        Task InitializePlayerAsync();
    }

    public class PlayData : IPlayData
    {
        #region Private fields and the constructor 

        private readonly IPlayerRepository _playerRepository;
        private readonly IRepository<Score> _scoreRepository;
        private readonly IRepository<AnswerAttempt> _answerAttemptRepository;
        private readonly ILoggingService _loggingService;
        private readonly IMessageService _messageService;

        public PlayData(IPlayerRepository playerRepository, IRepository<Score> scoreRepository, 
            IRepository<AnswerAttempt> answerAttemptRepository, ILoggingService loggingService, IMessageService messageService)
        {
            _playerRepository = playerRepository;
            _scoreRepository = scoreRepository;
            _answerAttemptRepository = answerAttemptRepository;
            _loggingService = loggingService;
            _messageService = messageService;
        }
        #endregion


        #region Public methods and properties 
        public async Task InitializePlayerAsync()
        {
            CurrentPlayer = await GetLastActivePlayerAsync();
        }

        public Player? CurrentPlayer { get; set; }

        #endregion


        #region Private methods 

        /// <summary>
        /// Returns the player that made the last answer attempt.<br/>
        /// Failing that, the player with the largest Id. (The last one added to the DB).<br/>
        /// Failing that, just creates a new player based on the OS username.
        /// </summary>
        /// <returns>The last active player (or a new player)</returns>
        private async Task<Player> GetLastActivePlayerAsync()
        {
            // TODO:
            // 1. Check the database - if there are AnswerAttempts, take PlayerId from the last answer attempt,
            // find the player in the DB and return it
            // 2. If Player not found by Id, log the error.



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
            Application.Current.Shutdown();
            throw new Exception(message);
        }
        #endregion

    }
}
