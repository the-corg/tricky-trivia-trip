using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// Base class for view models, provides INotifyPropertyChanged implementation to inheriting ViewModels
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
