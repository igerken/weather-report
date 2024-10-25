using WeatherReport.Core;

namespace WeatherReport.WinApp.Events;

public class WeatherUpdated(IWeatherInfo weather)
{
    public IWeatherInfo Weather => weather;
}
