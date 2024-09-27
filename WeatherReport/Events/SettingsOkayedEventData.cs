using WeatherReport.Core;
using WeatherReport.Core.Events;
using WeatherReport.WinApp.Data;

namespace WeatherReport.WinApp.Events;

public class SettingsOkayedEventData : ILocationChanged
{
    public ILocation Location { get; }

    public SettingsOkayedEventData(string countryCode, string city)
	{
		Location = new Location { Country = countryCode, City = city };
	}
}