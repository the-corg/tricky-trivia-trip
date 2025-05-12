namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Model class for a score with a player's name instead of Id
    /// </summary>
    public class ScoreWithPlayerName
    {
        public DateTime Timestamp { get; set; }
        public required string PlayerName { get; set; }
        public int Value { get; set; }
    }
}
