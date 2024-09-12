namespace WeatherReport.Core;

public class WeatherServiceException : Exception
{
    private WeatherServiceExceptionReason _reason;

    public WeatherServiceExceptionReason Reason { get { return _reason; } }

    public WeatherServiceException()
    {
        _reason = WeatherServiceExceptionReason.Unknown;
    }

    public WeatherServiceException(WeatherServiceExceptionReason reason, string message)
        : base(message)
    {
        _reason = reason;
    }

    public WeatherServiceException(WeatherServiceExceptionReason reason, string message, Exception innerException)
        : base(message, innerException)
    {
        _reason = reason;
    }
}