using TrickyTriviaTrip.Model;

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
        public Player CurrentPlayer { get; set; }
    }

    public class PlayData : IPlayData
    {
        private Player _currentPlayer = new Player() { Name = Environment.UserName };

        public Player CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer == value)
                    return;

                _currentPlayer = value;
            }
        }

    }
}
