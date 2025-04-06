namespace TrickyTriviaTrip.Model
{
    public class Score
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public int Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
