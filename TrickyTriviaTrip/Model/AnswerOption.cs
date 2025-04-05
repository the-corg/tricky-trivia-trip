namespace TrickyTriviaTrip
{
    public class AnswerOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
