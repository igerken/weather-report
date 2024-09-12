using WeatherReport.DataModel;

namespace WeatherReport.Interfaces
{
	public interface IGlobalDataContainer
	{
		YrLocationDataContainer YrLocationData { get; set; }
	}
}
