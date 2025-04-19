namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Combined model class for a Question together with its AnswerOptions
    /// </summary>
    public class QuestionWithOptions
    {
        public required Question Question { get; set; }
        public List<AnswerOption> Options { get; set; } = new();
    }
}
