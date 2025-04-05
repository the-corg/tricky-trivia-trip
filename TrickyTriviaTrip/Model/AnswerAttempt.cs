namespace TrickyTriviaTrip
{
    public class AnswerAttempt
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerOptionId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
