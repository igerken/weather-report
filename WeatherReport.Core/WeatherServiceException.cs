namespace WeatherReport.Core;

public class WeatherServiceException : Exception
{
    private WeatherServiceFailureReason _reason;

    public WeatherServiceFailureReason Reason { get { return _reason; } }

    public WeatherServiceException()
    {
        _reason = WeatherServiceFailureReason.Unknown;
    }

    public WeatherServiceException(WeatherServiceFailureReason reason, string message)
        : base(message)
    {
        _reason = reason;
    }

    public WeatherServiceException(WeatherServiceFailureReason reason, string message, Exception innerException)
        : base(message, innerException)
    {
        _reason = reason;
    }
}