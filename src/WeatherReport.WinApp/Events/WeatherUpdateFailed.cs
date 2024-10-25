using WeatherReport.Core;

namespace WeatherReport.WinApp.Events;

public class WeatherUpdateFailed(WeatherServiceFailureReason reason)
{
    public WeatherServiceFailureReason Reason => reason;
}
