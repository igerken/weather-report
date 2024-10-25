using WeatherReport.Core;

namespace WeatherReport.WinApp.Data;

public class Location : ILocation
{
    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}
