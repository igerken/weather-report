using WeatherReport.WinApp.Data;

namespace WeatherReport.WinApp.Events;

public class SettingsOkayed : LocationChanged
{
    public SettingsOkayed(string countryCode, string city) : base(new Location { Country = countryCode, City = city })
	{
	}
}