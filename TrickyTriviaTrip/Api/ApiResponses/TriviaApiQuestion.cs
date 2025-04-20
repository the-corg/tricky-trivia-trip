using System.Net;

namespace TrickyTriviaTrip.Api.ApiResponses
{
    /// <summary>
    /// Structure of one question from the API response to a request for questions.<br/>
    /// Decodes strings with HTML codes and stores only decoded strings.<br/>
    /// Is deserialized from JSON and returned to QuestionQueue for further conversion
    /// </summary>
    public class TriviaApiQuestion
    {
        private string _category = "";
        private string _question = "";
        private string _correctAnswer = "";
        private List<string> _incorrectAnswers = new();

        public required string Type { get; set; }
        public required string Difficulty { get; set; }

        public required string Category
        {
            get => _category;
            set => _category = WebUtility.HtmlDecode(value);
        }
        public required string Question
        {
            get => _question;
            set => _question = WebUtility.HtmlDecode(value);
        }
        public required string CorrectAnswer
        {
            get => _correctAnswer;
            set => _correctAnswer = WebUtility.HtmlDecode(value);
        }
        public List<string> IncorrectAnswers
        {
            get => _incorrectAnswers;
            set => _incorrectAnswers.AddRange(value.Select(x => WebUtility.HtmlDecode(x)));
        }
    }
}
