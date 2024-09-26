namespace WeatherReport.YrService.Contract;

public class TimeSeriesItem
{
    public DateTime Time { get; set; }
    public TimeSeriesItemData? Data { get; set; }
}