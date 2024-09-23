using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Infrastructure.DbInitializers;
using CasualTasker.Infrastructure.Middleware;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using CasualTasker.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace CasualTasker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? _hosting;

        /// <summary>
        /// Gets the hosting instance.
        /// </summary>
        public static IHost Hosting => _hosting ??=
            Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
            .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.json", true, true))
            .ConfigureLogging((context, log) =>
            {
                log.ClearProviders();
                log.AddSerilog(
                    new LoggerConfiguration()
                    .ReadFrom.Configuration(context.Configuration)
                    .CreateLogger()
                    );
            })
            .ConfigureServices(ConfigureServices)
            .Build();

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        public static IServiceProvider Services => Hosting.Services;

        /// <summary>
        /// Configures services for dependency injection.
        /// </summary>
        /// <param name="host">The host builder context.</param>
        /// <param name="services">The service collection.</param>
        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddDbContext<CasualTaskerDbContext>(opt =>
                opt.UseSqlite(host.Configuration.GetConnectionString("Sqlite"))
            );

            services.AddTransient<IStore<TaskDTO>, TaskStore>();
            services.AddTransient<IStore<CategoryDTO>, CategoryStore>();
            services.AddTransient<ICategoryFallbackService, CategoryFallbackService>();
            services.AddTransient<CasualTaskerDbInitializer>();

            services.AddSingleton<DataRepository>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<EditCategoryPageViewModel>();
            services.AddSingleton<EditTaskPageViewModel>();

            services.AddSingleton<IExceptionHandlingService, ExceptionHandlingService>();
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <param name="e">The startup event arguments.</param>
        protected async override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            SetCultureInfo("ru-RU");
            await Services.GetRequiredService<CasualTaskerDbInitializer>().InitializeAsync();
        }

        /// <summary>
        /// Handles unhandled exceptions that occur on the dispatcher thread.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionHandler = Services.GetRequiredService<IExceptionHandlingService>();
            exceptionHandler.HandleException(e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// Handles unhandled exceptions that occur outside the dispatcher thread.
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                var exceptionHandler = Services.GetRequiredService<IExceptionHandlingService>();
                exceptionHandler.HandleException(ex);
            }
        }

        /// <summary>
        /// Sets the culture info for the application.
        /// </summary>
        /// <param name="name">The name of the culture.</param>
        private void SetCultureInfo(string name)
        {
            var culture = new CultureInfo(name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        /// <summary>
        /// Creates a Serilog logger based on the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration to read from.</param>
        /// <returns>A configured Serilog logger.</returns>
        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }

}
