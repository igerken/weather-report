namespace WeatherReport.WinApp.Events;

public class SettingsOkayedEventData
{
	public string CountryCode { get; private set; }
	public string City { get; private set; }

	public SettingsOkayedEventData(string countryCode, string city)
	{
		CountryCode = countryCode;
		City = city;
	}
}