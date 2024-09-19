namespace WeatherReport.Core.Events;

public interface IWeatherUpdated
{
    IWeatherInfo Weather { get; }
}
