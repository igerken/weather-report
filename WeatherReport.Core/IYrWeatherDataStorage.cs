using WeatherReport.DataModel;

namespace WeatherReport.Interfaces
{
	public interface IYrWeatherDataStorage
	{
		bool IsLocationDataAvailable();
		YrLocationDataContainer GetLocationData();
		void StoreLocationData(YrLocationDataContainer locationData);
	}
}
