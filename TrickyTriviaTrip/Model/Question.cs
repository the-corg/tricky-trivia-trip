namespace TrickyTriviaTrip
{
    public class Question
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public required string CorrectAnswer { get; set; }
    }
}
