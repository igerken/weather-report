using WeatherReport.Core;

namespace WeatherReport.WinApp.Events;

public class LocationChanged(ILocation location)
{
    public ILocation Location => location;
}
