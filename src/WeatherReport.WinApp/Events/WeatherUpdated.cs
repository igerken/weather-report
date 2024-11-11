using WeatherReport.Core;

namespace WeatherReport.WinApp.Events;

public class WeatherUpdated(IWeatherInfo weather)
{
    public IWeatherInfo Weather => weather;

    public WeatherUpdated(double? temperature, double? windSpeed, double? windDirection) : this(new WeatherInfo(temperature, windSpeed, windDirection))
    {
    }

    private class WeatherInfo(double? temperature, double? windSpeed, double? windDirection) : IWeatherInfo
    {
        public double? Temperature => temperature;

        public double? WindDirection => windDirection;

        public double? WindSpeed => windSpeed;
    }
}
