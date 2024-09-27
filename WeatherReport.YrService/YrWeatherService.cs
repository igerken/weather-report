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
        LocationSettings? locationSettings = _locationSettingsOptions.Value.FirstOrDefault(
            ls => ls.Country == location.Country && ls.City == location.City);
        if(locationSettings != null)
        {
            string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={locationSettings.Lat}&lon={locationSettings.Long}";
            CompactResponse? response = 
                await _httpClient.GetFromJsonAsync<CompactResponse>(url, _jsonSerializerOptions);
            Details? details = response?.Properties?.Timeseries?.FirstOrDefault()?.Data?.Instant?.Details;
            if(details != null)
            {
                double windDirection = Math.PI * (double)details.WindFromDirection / 180.0;
                return new YrWeatherInfo((double)details.AirTemperature, windDirection, (double)details.WindSpeed);
            }
        }

        return YrWeatherInfo.Empty;
    }
}