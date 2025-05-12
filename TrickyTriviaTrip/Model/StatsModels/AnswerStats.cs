using TrickyTriviaTrip.DataAccess;

namespace TrickyTriviaTrip.Model
{
    /// <summary>
    /// Model class for answer stats for a particular criterion (category or difficulty) 
    /// </summary>
    public class AnswerStats
    {
        /// <summary>
        /// Shows whether this object describes stats
        /// for a Category or a Difficulty
        /// </summary>
        public required Criterion Criterion { get; set; }

        /// <summary>
        /// The text for the actual category or difficulty
        /// ("Easy", "Geography", etc.)
        /// </summary>
        public required string CriterionText { get; set; }

        public int TotalAnswered { get; set; }
        public int CorrectlyAnswered { get; set; }
        public double CorrectPercentage { get; set; }
    }
}
