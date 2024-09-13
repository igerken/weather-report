using System.Windows;
using Caliburn.Micro;

using WeatherReport.Events;
using WeatherReport.Interfaces;

namespace WeatherReport.WinApp.ViewModels;

public class DownloadProgressViewModel : PropertyChangedBase, IHandle<DownloadRequestedEventData>, IHandle<DownloadProgressChangedEventData>
{
	private const double FULL_DOWNLOAD_PROGRESS_ANGLE = 1.5 * Math.PI;
	private const double DOWNLOAD_PROGRESS_ANGLE_START = 0.25 * Math.PI;
	private const double DOWNLOAD_PROGRESS_R = 30.0;

	private readonly IYrWeatherService _yrWeatherService;
	private readonly IGlobalDataContainer _globalDataContainer;
	private readonly IEventAggregator _eventAggregator;

	private bool _isLargeArc = false;
	private bool _isIndicatorVisible = false;
	private Point _downloadProgressEndPoint;

	public bool IsLargeArc
	{
		get { return _isLargeArc; }
		set
		{
			if (_isLargeArc != value)
			{
				_isLargeArc = value;
				NotifyOfPropertyChange(() => IsLargeArc);
			}
		}
	}

	public Point DownloadProgressEndPoint
	{
		get { return _downloadProgressEndPoint; }
		set
		{
			if (_downloadProgressEndPoint != value)
			{
				_downloadProgressEndPoint = value;
				NotifyOfPropertyChange(() => DownloadProgressEndPoint);
			}
		}
	}

	public bool IsIndicatorVisible
	{
		get { return _isIndicatorVisible; }
		set
		{
			if (_isIndicatorVisible != value)
			{
				_isIndicatorVisible = value;
				NotifyOfPropertyChange(() => IsIndicatorVisible);
			}
		}
	}

	public DownloadProgressViewModel(IYrWeatherService yrWeatherService, IGlobalDataContainer globalDataContainer, IEventAggregator eventAggregator)
	{
		_yrWeatherService = yrWeatherService;
		_globalDataContainer = globalDataContainer;
		_eventAggregator = eventAggregator;
		_eventAggregator.Subscribe(this);
	}

    public Task HandleAsync(DownloadRequestedEventData message, CancellationToken cancellationToken)
	{
		IsIndicatorVisible = false;
		return Task.CompletedTask;
	}

    public Task HandleAsync(DownloadProgressChangedEventData message, CancellationToken cancellationToken)
	{
		if (message.Data.ExpectedTotalSize.HasValue)
		{
			IsIndicatorVisible = true;
			float downloadCompleteness = ((float)message.Data.DownloadedSize) / ((float)message.Data.ExpectedTotalSize.Value);
			double dpx = 28.8;
			double dpy = 71.2;
			double progressAngle = DOWNLOAD_PROGRESS_ANGLE_START + downloadCompleteness * FULL_DOWNLOAD_PROGRESS_ANGLE;
			dpx = 50.0 - DOWNLOAD_PROGRESS_R * Math.Sin(progressAngle);
			dpy = 50.0 + DOWNLOAD_PROGRESS_R * Math.Cos(progressAngle);
			DownloadProgressEndPoint = new Point(dpx, dpy);
			IsLargeArc = downloadCompleteness > 0.5;
		}
		return Task.CompletedTask;
	}
}