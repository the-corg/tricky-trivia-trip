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
        public string Text
        {
            get => _model.Text;
            set
            {
                if (value == _model.Text)
                    return;

                _model.Text = value;
                OnPropertyChanged();
            }
        }

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
            }
        }
        #endregion

    }
}
