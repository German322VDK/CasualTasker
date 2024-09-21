using CasualTasker.Database.Context;
using CasualTasker.Infrastructure.Middleware;
using CasualTasker.Infrastructure.ObservableDbCollections;
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

        public static IServiceProvider Services => Hosting.Services;

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddDbContext<CasualTaskerDbContext>(opt =>
                opt.UseSqlite(host.Configuration.GetConnectionString("Sqlite"))
            );
            services.AddSingleton<DataRepository>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<EditCategoryPageViewModel>();
            services.AddSingleton<EditTaskPageViewModel>();

            services.AddSingleton<IExceptionHandlingService, ExceptionHandlingService>();
        }


        protected async override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            SetCultureInfo("ru-RU");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionHandler = Services.GetRequiredService<IExceptionHandlingService>();
            exceptionHandler.HandleException(e.Exception);
            e.Handled = true;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                var exceptionHandler = Services.GetRequiredService<IExceptionHandlingService>();
                exceptionHandler.HandleException(ex);
            }
        }

        private void SetCultureInfo(string name)
        {
            var culture = new CultureInfo(name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }

}
