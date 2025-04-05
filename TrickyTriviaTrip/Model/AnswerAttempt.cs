namespace TrickyTriviaTrip
{
    public class AnswerAttempt
    {
        public int PlayerId { get; set; }
        public int QuestionId { get; set; }
        public DateTime DateTime { get; set; }
        public string? Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
