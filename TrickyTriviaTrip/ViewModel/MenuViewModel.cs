namespace TrickyTriviaTrip.ViewModel
{
    /// <summary>
    /// View Model for MenuView
    /// </summary>
    public class MenuViewModel : BaseViewModel
    {

        public string? PlayerName => Environment.UserName;
    }
}
