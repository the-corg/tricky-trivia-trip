namespace TrickyTriviaTrip.Model
{
    public class AnswerOption
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
