using System.Windows;
using System.Windows.Input;
using TrickyTriviaTrip.ViewModel;

namespace TrickyTriviaTrip
{
    /// <summary>
    /// MVVM-friendly code-behind for MainWindow.xaml.
    /// For the most part, it's strictly view-related code.
    /// Additionally, one DataContext method is called, to handle startup
    /// (initialize data asynchronously after the UI is loaded) to avoid
    /// creating extra complexity of attached behaviors for silly reasons.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // Calls the view model's initializer.
        // To decrease the app startup time, it's important to do
        // asynchronous initialization only after the UI is loaded 
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                await mainViewModel.InitializeAsync();
            }
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