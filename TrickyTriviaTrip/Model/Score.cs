namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Model class for the score of a game session
    /// </summary>
    public class Score
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public int Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
