using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WeatherReport.Core;
using WeatherReport.Core.Events;
using WeatherReport.WinApp.Data;

namespace WeatherReport.WinApp;

public class WeatherUpdateService : IHandle<ILocationChanged>
{
    private readonly IWeatherService _weatherService;

    private readonly IEventAggregator _eventAggregator;

    private readonly ILogger<WeatherUpdateService> _logger;

    private readonly IOptions<AppSettings> _appSettings;
    
	private readonly System.Timers.Timer _weatherInfoUpdateTimer;

    private ILocation? _location;

    public WeatherUpdateService(IWeatherService weatherService, IEventAggregator eventAggregator,
        IOptions<AppSettings> appSettings, ILogger<WeatherUpdateService> logger)
    {
		_weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
		_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _logger = logger;

        _weatherInfoUpdateTimer = new System.Timers.Timer(_appSettings.Value.RefreshIntervalSeconds*1000.0);
        _weatherInfoUpdateTimer.Elapsed += (s, e) => UpdateWeather();
        _weatherInfoUpdateTimer.AutoReset = true;
        _weatherInfoUpdateTimer.Enabled = true;
        
		_eventAggregator.SubscribeOnPublishedThread(this);
    }

    public Task HandleAsync(ILocationChanged message, CancellationToken cancellationToken)
    {
        _location = message.Location;
        UpdateWeather();
        return Task.CompletedTask;
    }

    private async void UpdateWeather()
    {
        try
        {
            if(_location == null)
                return;

            var winfo = await _weatherService.GetWeather(_location);
            await _eventAggregator.PublishOnUIThreadAsync(new WeatherUpdated(winfo));
        }
        catch (WeatherServiceException wsex)
        {
            _logger?.LogError(wsex, "Failed to retrieve weather");
            await _eventAggregator.PublishOnUIThreadAsync(new WeatherUpdateFailed(wsex.Reason));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to retrieve weather");
            await _eventAggregator.PublishOnUIThreadAsync(new WeatherUpdateFailed(WeatherServiceFailureReason.WeatherInfoUnavailable));
        }
    }

    private class WeatherUpdated(IWeatherInfo weather) : IWeatherUpdated
    {
        public IWeatherInfo Weather => weather;
    }

    private class WeatherUpdateFailed(WeatherServiceFailureReason reason) : IWeatherUpdateFailed
    {
        public WeatherServiceFailureReason Reason => reason;
    }
}