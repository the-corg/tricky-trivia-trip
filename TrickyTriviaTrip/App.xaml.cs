using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TrickyTriviaTrip.Api;
using TrickyTriviaTrip.DataAccess;
using TrickyTriviaTrip.GameLogic;
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
        #region Constructor and private fields 

        private readonly ServiceProvider _serviceProvider;
        private readonly ILoggingService _loggingService;

        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
            ConfigureExceptionHandling();
        }

        #endregion

        #region Global exception handling
        private void ConfigureExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception exception)
                    _loggingService.LogError("Unhandled exception (AppDomain):\n" + exception.ToString());
            };

            DispatcherUnhandledException += (s, e) =>
            {
                _loggingService.LogError("Unhandled exception (Dispatcher):\n" + e.Exception.ToString());
                //e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                _loggingService.LogError("Unobserved exception (TaskScheduler):\n" + e.Exception.ToString());
                e.SetObserved();
            };
        }
        #endregion


        #region Configure services for dependency injection
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            // View Models
            services.AddSingleton<MainViewModel>();
            services.AddTransient<MenuViewModel>();
            services.AddTransient<GameViewModel>();
            services.AddTransient<StatsViewModel>();

            // API
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ITriviaApiClient, TriviaApiClient>();

            // Database
            services.AddSingleton<IDatabaseConfig, DatabaseConfig>();
            services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
            services.AddSingleton<IDatabaseInitializer, SqliteDatabaseInitializer>();

            // Repositories
            services.AddSingleton<IPlayerRepository, PlayerRepository>();
            services.AddSingleton<IQuestionRepository, QuestionRepository>();
            services.AddSingleton<IAnswerOptionRepository, AnswerOptionRepository>();
            services.AddSingleton<IScoreRepository, ScoreRepository>();
            services.AddSingleton<IAnswerAttemptRepository, AnswerAttemptRepository>();

            // Game logic
            services.AddSingleton<IPlayData, PlayData>();
            services.AddSingleton<IQuestionQueue, QuestionQueue>();

            // Other
            services.AddTransient<IMessageService, MessageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ILoggingService, LoggingService>();
        }
        #endregion

        #region OnStartup and OnExit
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Show the main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow?.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Will dispose of all disposables among services (e.g., ILoggingService)
            _serviceProvider.Dispose();

            base.OnExit(e);
        }
        #endregion
    }

}
