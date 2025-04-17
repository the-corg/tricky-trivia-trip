namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Model class for one of the answer options of a question
    /// </summary>
    public class AnswerOption
    {
        public long Id { get; set; }
        public long QuestionId { get; set; }
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
