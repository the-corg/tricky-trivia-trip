namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Combined model class for a Question together with its AnswerOptions
    /// </summary>
    public class QuestionWithAnswers
    {
        public required Question Question { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; } = new();
    }
}
