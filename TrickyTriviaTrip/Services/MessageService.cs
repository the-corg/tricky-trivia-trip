using System.Windows;

namespace TrickyTriviaTrip.Services
{
    /// <summary>
    /// Allows showing message boxes to the user
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Shows a standard message box (mainly for errors)
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="caption">Message title</param>
        void ShowMessage(string message, string caption = "Error");
        /// <summary>
        /// Shows a confirmation dialog box with Yes and No buttons
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="caption">Message title</param>
        /// <returns>True, if the user clicked <strong>Yes</strong><br/>False, otherwise</returns>
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
