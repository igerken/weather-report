namespace WeatherReport.Core
{
    public enum WeatherServiceExceptionReason
    {
        Unknown = 0,
        CityListUnavailable,
        CityListInvalidData,
        WeatherInfoUnavailable,
        WeatherInfoInvalidData
    }
}
