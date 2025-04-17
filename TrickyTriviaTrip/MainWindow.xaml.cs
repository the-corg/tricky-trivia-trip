using System.Windows;
using System.Windows.Input;

namespace TrickyTriviaTrip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Handling window state changes
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Increase the window border thickness for the maximized state,
                // otherwise the window would extend beyond the screen edges
                BorderThickness = new Thickness(8);
                MaximizeButton.Visibility = Visibility.Collapsed;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        private void CommandBinding_MinimizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CommandBinding_MaximizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void CommandBinding_RestoreExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void CommandBinding_CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion

    }
}