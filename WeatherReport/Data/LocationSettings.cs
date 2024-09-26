namespace WeatherReport.Data;

public class LocationSettings
{
    public string Country { get; set; } = string.Empty;
    
    public string City { get; set; } = string.Empty;
    
    public float Lat { get; set; }
    
    public float Long { get; set; }
}
