using Microsoft.Extensions.DependencyInjection;
using WeatherReport.Core;

namespace WeatherReport.YrService;

public static class Configurator
{
    public static IServiceCollection AddYrWeatherService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IWeatherService, YrWeatherService>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "WeatherReport");
        });
        return serviceCollection;
    }
}
