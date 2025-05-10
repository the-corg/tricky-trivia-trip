using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View model for a single answer option
    /// </summary>
    public class AnswerViewModel : BaseViewModel
    {
        #region Private fields and the constructor

        private AnswerOption _model;
        private bool _isSelected;

        public AnswerViewModel(AnswerOption model)
        {
            _model = model;
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Text of the answer option
        /// </summary>
        public string Text => _model.Text;

        /// <summary>
        /// Is shown to the left of the answer text after an answer option is selected
        /// </summary>
        public string LeftCorrectnessDecorator => IsSelected ? (IsCorrect ? "✔️⇨ " : "❌⇨ ") : "";

        /// <summary>
        /// Is shown to the right of the answer text after an answer option is selected
        /// </summary>
        public string RightCorrectnessDecorator => IsSelected ? (IsCorrect ? " ⇦✔️" : " ⇦❌") : "";

        /// <summary>
        /// Shows whether the answer option is correct
        /// </summary>
        public bool IsCorrect => _model.IsCorrect;

        /// <summary>
        /// Shows whether the answer option was selected by the user
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LeftCorrectnessDecorator));
                OnPropertyChanged(nameof(RightCorrectnessDecorator));
            }
        }

        /// <summary>
        /// The original answer option object 
        /// </summary>
        public AnswerOption Model => _model;

        #endregion

    }
}
