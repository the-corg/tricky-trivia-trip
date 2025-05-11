using TrickyTriviaTrip.Model;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View model for a single player
    /// </summary>
    public class PlayerViewModel : BaseViewModel
    {
        #region Private fields and the constructor

        private Player? _model;
        private bool _isCurrent;

        public PlayerViewModel(Player? model, bool isDummy = false)
        {
            _model = model;
            IsDummy = isDummy;
        }
        #endregion


        #region Public properties

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name
        {
            get => IsDummy ? "Add New Player..." : _model!.Name;
            set
            {
                if (IsDummy || value == _model!.Name)
                    return;

                _model!.Name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Id of the player
        /// </summary>
        public long Id => IsDummy ? -1 : _model!.Id;

        /// <summary>
        /// Is true if and only if the player is the current player
        /// (for sorting)
        /// </summary>
        public bool IsCurrent
        {
            get => _isCurrent;
            set
            {
                if (_isCurrent == value) 
                    return;

                _isCurrent = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Is true for the dummy player item
        /// </summary>
        public bool IsDummy { get; }

        /// <summary>
        /// The original player object 
        /// </summary>
        public Player? Model => _model;

        #endregion
    }
}
