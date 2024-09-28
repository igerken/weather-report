using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Caliburn.Micro;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;
using Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro;
using WeatherReport.WinApp.ViewModels;
using WeatherReport.Core;
using WeatherReport.Core.Settings;
using WeatherReport.YrService;
using WeatherReport.Data;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.WinApp;

public static class Program
{
    private const string AppSettingsFilePrefix = "appsettings";
    private const string HostSettingsFile = "hostsettings.json";
    private const string Prefix = "PREFIX_";

    public static Task Main(string[] args)
    {
        var executableLocation = Path.GetDirectoryName(typeof(Program).Assembly.Location);
           
        var host = new HostBuilder()
            .ConfigureWpf()
            .ConfigureLogging()
            .ConfigureConfiguration(args)
            .ConfigureCaliburnMicro<MainViewModel>()
            .ConfigureServices((ctx, serviceCollection) =>
            {
                serviceCollection.AddTransient<DownloadProgressViewModel>();
                serviceCollection.AddTransient<UserSettingsViewModel>();
                serviceCollection.AddSingleton<WindDirectionViewModel>();
                serviceCollection.AddSingleton<WeatherUpdateService>();
                serviceCollection.AddSingleton<IWindowManager, WindowManager>();
                serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();
                serviceCollection.AddSingleton<IUserSettings>(new UserSettings());
                serviceCollection.AddOptions<List<LocationSettings>>().Bind(ctx.Configuration.GetSection(nameof(LocationSettings)));
                serviceCollection.AddYrWeatherService();
            })
            .UseConsoleLifetime()
            .UseWpfLifetime()
            .Build();

        Console.WriteLine("Run!");
        host.Services.GetRequiredService<WeatherUpdateService>();
        return host.RunAsync();
    }

    /// <summary>
    /// Configure the loggers
    /// </summary>
    /// <param name="hostBuilder">IHostBuilder</param>
    /// <returns>IHostBuilder</returns>
    private static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureLogging((hostContext, configLogging) =>
        {
            configLogging
                .AddConfiguration(hostContext.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug();
        });
    }

    /// <summary>
    /// Configure the configuration
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private static IHostBuilder ConfigureConfiguration(this IHostBuilder hostBuilder, string[] args)
    {
        return hostBuilder.ConfigureHostConfiguration(configHost =>
            {
                configHost
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(HostSettingsFile, optional: true)
                    .AddEnvironmentVariables(prefix: Prefix)
                    .AddCommandLine(args);
            })
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp
                    .AddJsonFile(AppSettingsFilePrefix + ".json", optional: true)
                    .AddEnvironmentVariables(prefix: Prefix)
                    .AddCommandLine(args);
                if (!string.IsNullOrEmpty(hostContext.HostingEnvironment.EnvironmentName))
                {
                    configApp.AddJsonFile(AppSettingsFilePrefix + $".{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                }
            });
    }
}