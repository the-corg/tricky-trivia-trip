namespace TrickyTriviaTrip.Model
{
    public class AnswerAttempt
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public long QuestionId { get; set; }
        public long AnswerOptionId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
