using System.Windows;

namespace TrickyTriviaTrip.Services
{
    /// <summary>
    /// Allows showing message boxes to the user
    /// </summary>
    public interface IMessageService
    {
        void ShowMessage(string message, string caption = "Error");
        bool ShowConfirmation(string message, string caption = "Please Confirm");
    }

    public class MessageService : IMessageService
    {
        public void ShowMessage(string message, string caption = "Error")
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirmation(string message, string caption = "Please Confirm")
        {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNo, 
                MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes;
        }
    }
}
