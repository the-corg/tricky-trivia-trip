using System.Windows.Controls;

namespace TrickyTriviaTrip.Services
{
    /// <summary>
    /// Manages navigation between the views
    /// </summary>
    public interface INavigationService
    {
        UserControl CurrentView { get; }
        void NavigateToMenu();
        void NavigateToGame();
        void NavigateToStats();
    }

    public class NavigationService : INavigationService
    {
        public NavigationService() { }
        public UserControl CurrentView { get; private set; }

        public void NavigateToGame()
        {
            throw new NotImplementedException();
        }

        public void NavigateToMenu()
        {
            throw new NotImplementedException();
        }

        public void NavigateToStats()
        {
            throw new NotImplementedException();
        }
    }
}
