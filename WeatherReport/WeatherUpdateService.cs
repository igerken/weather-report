using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using WeatherReport.Core;
using WeatherReport.Core.Events;

namespace WeatherReport;

public class WeatherUpdateService : IHandle<ILocationChanged>
{
    private readonly IWeatherService _weatherService;

    private readonly IEventAggregator _eventAggregator;

    private readonly ILogger<WeatherUpdateService> _logger;
    
	private readonly System.Timers.Timer _weatherInfoUpdateTimer;

    private ILocation? _location;

    public WeatherUpdateService(IWeatherService weatherService, IEventAggregator eventAggregator,
        ILogger<WeatherUpdateService> logger)
    {
		_weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
		_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _logger = logger;

        _location = new DummyLocation();

        _weatherInfoUpdateTimer = new System.Timers.Timer(2000);
        _weatherInfoUpdateTimer.Elapsed += (s, e) => UpdateWeather();
        _weatherInfoUpdateTimer.AutoReset = true;
        _weatherInfoUpdateTimer.Enabled = true;
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
            await _eventAggregator.PublishOnUIThreadAsync(new WetherUpdated(winfo));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to retrieve weather");
        }
    }

    private class WetherUpdated(IWeatherInfo weather) : IWeatherUpdated
    {
        public IWeatherInfo Weather => weather;
    }

    private class DummyLocation : ILocation
    {
        public string Country => "";

        public string City => "";
    }
}