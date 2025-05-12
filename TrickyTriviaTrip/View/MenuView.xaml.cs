using System.Windows.Controls;
using System.Windows.Threading;

namespace TrickyTriviaTrip.View
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        #region VisibleChanged event handlers for TextBox and ComboBox 

        private void TextBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // If the text box became visible, set the keyboard focus in it, put the cursor at the end and scroll there
            if (e.NewValue is bool b && b)
            {
                PlayerNameTextBox.Select(PlayerNameTextBox.Text.Length, 0);
                PlayerNameTextBox.ScrollToHorizontalOffset(double.MaxValue);

                // Have to use Dispatcher because otherwise the text box would lose focus to MainWindow 
                // when becoming visible after a click on "Create new player" button. This click would
                // be processed later than the visibility change for the TextBox
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, () => PlayerNameTextBox.Focus());
            }
        }

        private void ComboBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // If the combobox became visible, open the dropdown, otherwise close it
            if (e.NewValue is bool b && b)
                PlayersComboBox.IsDropDownOpen = true;
            else
                PlayersComboBox.IsDropDownOpen = false;
        }
        #endregion
    }
}
