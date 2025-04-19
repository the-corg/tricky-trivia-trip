using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrickyTriviaTrip.Services;

namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// Base class for view models, provides INotifyPropertyChanged implementation to inheriting ViewModels
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected readonly INavigationService _navigationService;

        protected BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
