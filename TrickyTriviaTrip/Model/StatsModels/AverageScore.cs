namespace TrickyTriviaTrip.Model
{
    public class AverageScore
    {
        public required string PlayerName { get; set; }
        public double Value { get; set; }
        public int NumberOfGames { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
