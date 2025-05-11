using System.Windows.Controls;

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

        private void TextBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // If the text box became visible, set the keyboard focus in it, put cursor at the end and scroll there
            if (e.NewValue is bool b && b)
            {
                PlayerNameTextBox.Focus();
                PlayerNameTextBox.Select(PlayerNameTextBox.Text.Length, 0);
                PlayerNameTextBox.ScrollToHorizontalOffset(double.MaxValue);
            }
        }

        private void ComboBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // If the combobox became visible, open the dropdown, otherwise close it
            if (e.NewValue is bool b && b)
            {
                PlayersComboBox.IsDropDownOpen = true;
            }
            else
            {
                PlayersComboBox.IsDropDownOpen = false;
            }
        }

    }
}
