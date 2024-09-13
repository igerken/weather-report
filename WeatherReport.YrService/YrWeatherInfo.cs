using WeatherReport.Core;

namespace WeatherReport.YrService;

internal class YrWeatherInfo : IWeatherInfo
{
    public double? Temperature { get; }

    public double? WindDirection { get; }

    public double? WindSpeed { get; }

    public YrWeatherInfo(double? temperature, double? windDirection, double? windSpeed)
    {
        Temperature = temperature;
        WindDirection = windDirection;
        WindSpeed = windSpeed;
    }
}