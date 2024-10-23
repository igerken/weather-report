using WeatherReport.WinApp.Data;

namespace WeatherReport.WinApp.Events;

public class SettingsOkayedEventData : LocationChanged
{
    public SettingsOkayedEventData(string countryCode, string city) : base(new Location { Country = countryCode, City = city })
	{
	}
}