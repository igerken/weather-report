using System;
using System.Threading.Tasks;
using WeatherReport.DataModel;

namespace WeatherReport.Interfaces
{
	public interface IYrWeatherService
	{
		Task<YrLocationDataContainer> GetLocationData(IProgress<DownloadProgressData> progress);
		Task<WeatherInfo> GetWeatherInfo(string weatherInfoUrl, IProgress<DownloadProgressData> progress = null);
	}
}
