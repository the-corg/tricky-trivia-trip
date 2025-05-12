namespace TrickyTriviaTrip.Model
{
    public class ScoreWithPlayerName
    {
        public DateTime Timestamp { get; set; }
        public required string PlayerName { get; set; }
        public int Value { get; set; }
    }
}
