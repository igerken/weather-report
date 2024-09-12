namespace WeatherReport.Data
{
	public class DownloadProgressData
	{
		private long? _expectedTotalSize;
		private long _downloadedSize;

		public long DownloadedSize { get { return _downloadedSize; } }
		public long? ExpectedTotalSize { get { return _expectedTotalSize; } }

		public DownloadProgressData() : this(0L, null) { }
		public DownloadProgressData(long downloadedSize, long? expectedTotalSize)
		{
			_downloadedSize = downloadedSize;
			_expectedTotalSize = expectedTotalSize;
		}
	}
}
