namespace WeatherReport.Core;

public interface IWeatherInfo
{
    /// <summary>Gets the temperature in °C</summary>
    double? Temperature { get; }

    /// <summary>Gets the wind direction</summary>
    double? WindDirection { get; }

    /// <summary>Gets the wind speed in m/s</summary>
    double? WindSpeed { get; }
}
