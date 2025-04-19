using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TrickyTriviaTrip.Api;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.Model;
using TrickyTriviaTrip.Services;
using TrickyTriviaTrip.ViewModel;

namespace TrickyTriviaTrip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        #region Configure services for dependency injection
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            // View Models
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<GameViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddSingleton<StatsViewModel>();

            // API
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ITriviaApiClient, TriviaApiClient>();

            // Database
            services.AddSingleton<IDatabaseConfig, DatabaseConfig>();
            services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
            services.AddSingleton<IDatabaseInitializer, SqliteDatabaseInitializer>();

            // Repositories
            services.AddTransient<IRepository<Player>, PlayerRepository>();
            services.AddTransient<IRepository<Question>, QuestionRepository>();
            services.AddTransient<IRepository<AnswerOption>, AnswerOptionRepository>();
            services.AddTransient<IRepository<Score>, ScoreRepository>();
            services.AddTransient<IRepository<AnswerAttempt>, AnswerAttemptRepository>();

            // Other
            services.AddTransient<IMessageService, MessageService>();
            services.AddSingleton<INavigationService, NavigationService>();

        }
        #endregion

        #region OnStartup (initialize everything and show Main Window)
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the database if it doesn't exist
            var dbInitializer = _serviceProvider.GetRequiredService<IDatabaseInitializer>();
            await dbInitializer.InitializeIfMissingAsync();

            // Show the main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow?.Show();
        }
        #endregion
    }

}
