namespace WeatherReport.Core.Events;

public interface ILocationChanged
{
    ILocation Location { get; }
}
