using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.ViewModel
{
    public class AnswerViewModel : BaseViewModel
    {
        private AnswerOption _model;
        private bool _isSelected;

        public AnswerViewModel(AnswerOption model)
        {
            _model = model;
        }

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

    }
}
