using WeatherReport.Data;

namespace WeatherReport.WinApp.Events;

public class DownloadProgressChangedEventData
{
	private DownloadProgressData _data;

	public DownloadProgressData Data { get { return _data; } }

	public DownloadProgressChangedEventData(DownloadProgressData data)
	{
		_data = data;
	}
}