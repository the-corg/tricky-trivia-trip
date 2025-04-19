namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Model class for a trivia question
    /// </summary>
    public class Question
    {
        public long Id { get; set; }
        public required string Text { get; set; }
    }
}
