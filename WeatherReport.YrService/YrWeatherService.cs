using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using WeatherReport.Core;
using WeatherReport.Core.Settings;
using Microsoft.Extensions.Options;
using WeatherReport.YrService.Contract;

namespace WeatherReport.YrService;

public class YrWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    private readonly IOptions<List<LocationSettings>> _locationSettingsOptions;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public YrWeatherService(HttpClient httpClient, IOptions<List<LocationSettings>> locationSettingsOptions)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		_locationSettingsOptions = locationSettingsOptions ?? throw new ArgumentNullException(nameof(locationSettingsOptions));

        _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };      
    }

    public async Task<IWeatherInfo> GetWeather(ILocation location)
    {
        //https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=50.0875&lon=14.4214
        //_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "WeatherReport");
        CompactResponse response = 
            await _httpClient.GetFromJsonAsync<CompactResponse>("https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=50.0875&lon=14.4214", _jsonSerializerOptions);
        Details? details = response?.Properties?.Timeseries?.FirstOrDefault()?.Data?.Instant?.Details;
        if(details != null)
        {
            return new YrWeatherInfo((double)details.AirTemperature, 3.0, (double)details.WindSpeed);
        }

        return new YrWeatherInfo(20.0, 3.0, 5.0);
    }
}