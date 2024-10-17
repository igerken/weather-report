namespace WeatherReport.Core.Events;

public interface IWeatherUpdateFailed
{
    WeatherServiceFailureReason Reason { get; }
}
