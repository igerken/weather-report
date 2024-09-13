using WeatherReport.Core;

namespace WeatherReport.YrService;

public class YrWeatherService : IWeatherService
{
    public async Task<IWeatherInfo> GetWeather(ILocation location)
    {
        await Task.Delay(500);
        return new YrWeatherInfo(20.0, 3.0, 5.0);
    }
}