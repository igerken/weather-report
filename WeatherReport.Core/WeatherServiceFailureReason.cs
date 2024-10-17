namespace WeatherReport.Core
{
    public enum WeatherServiceFailureReason
    {
        Unknown = 0,
        WeatherInfoUnavailable,
        WeatherInfoInvalidData,
        AccessDenied
    }
}
