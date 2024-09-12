namespace WeatherReport.Core;

public interface IWeatherService
{
    Task<IWeatherInfo> GetWeather(ILocation location);
}

