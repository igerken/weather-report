using System.Windows.Input;

using Caliburn.Micro;
using Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro;
using Microsoft.Extensions.Logging;
using WeatherReport.WinApp.Interfaces;
using WeatherReport.WinApp.Data;
using WeatherReport.WinApp.Events;
using WeatherReport.Core;
using WeatherReport.MVVM;
using WeatherReport.Core.Events;
using Dapplo.Microsoft.Extensions.Hosting.Wpf;

namespace WeatherReport.WinApp.ViewModels;

public class MainViewModel : Screen, ICaliburnMicroShell,
	IHandle<SettingsOkayedEventData>, IHandle<SettingsCancelledEventData>, IHandle<IWeatherUpdated>, IHandle<ILocationChanged>
{
	public const string PROP_WEATHER_INFO = "WeatherInfo";

	private const string DEFAULT_DISPLAYED_CITY = "----";        

	private readonly IWpfContext _wpfContext;

    private readonly IWindowManager _windowManager;

	private readonly IServiceProvider _serviceProvider;

	private readonly IEventAggregator _eventAggregator;
	
	private readonly ILogger<MainViewModel> _logger;

	private readonly IUserSettings _userSettings;

	private bool _isSettingsLayerVisible = false;
	private bool _isDownloadProgressLayerVisible;

	private RelayCommand _settingsCommand;

	private IWeatherInfo _weatherInfo;
	private string _displayedCity;
	private string _infoDisplayString = DEFAULT_DISPLAYED_CITY;

	private InfoDisplayStatus _infoDisplayStatus = InfoDisplayStatus.Normal;

	private AppSettings _settings;

	public WindDirectionViewModel WindDirectionViewModel =>
		(WindDirectionViewModel)_serviceProvider.GetService(typeof(WindDirectionViewModel))!;

	public UserSettingsViewModel UserSettingsViewModel =>
		(UserSettingsViewModel)_serviceProvider.GetService(typeof(UserSettingsViewModel))!;

	public IWeatherInfo WeatherInfo
	{
		get { return _weatherInfo; }
		set
		{
			if (!_weatherInfo.Equals(value))
			{
				_weatherInfo = value;
				NotifyOfPropertyChange(() => WeatherInfo);
				NotifyOfPropertyChange(() => TemperatureString);
			}
		}
	}

	public string DisplayedCity
	{
		get { return _displayedCity; }
		set
		{
			if (_displayedCity != value)
			{
				_displayedCity = value;
				NotifyOfPropertyChange(() => DisplayedCity);
			}
		}
	}

	public bool IsSettingsLayerVisible
	{
		get { return _isSettingsLayerVisible; }
		set
		{
			if (_isSettingsLayerVisible != value)
			{
				_isSettingsLayerVisible = value;
				NotifyOfPropertyChange(() => IsSettingsLayerVisible);
				NotifyOfPropertyChange(() => IsSettingsButtonVisible);
			}
		}
	}

	public bool IsDownloadProgressLayerVisible
	{
		get { return _isDownloadProgressLayerVisible; }
		set
		{
			if (_isDownloadProgressLayerVisible != value)
			{
				_isDownloadProgressLayerVisible = value;
				NotifyOfPropertyChange(() => IsDownloadProgressLayerVisible);
			}
		}
	}

	public bool IsSettingsButtonVisible
	{
		get { return !_isSettingsLayerVisible; }
	}

	public string TemperatureString
	{
		get
		{
			return _weatherInfo.Temperature.HasValue ?
                string.Format("{0,4:0.0}°C", _weatherInfo.Temperature.Value) : "--.-°C";
		}
	}

	public string InfoDisplayString
	{
		get { return _infoDisplayString; }
		set
		{
			if (_infoDisplayString != value)
			{
				_infoDisplayString = value;
				NotifyOfPropertyChange(() => InfoDisplayString);
			}
		}
	}

	public InfoDisplayStatus InfoDisplayStatus
	{
		get { return _infoDisplayStatus; }
		set
		{
			if (_infoDisplayStatus != value)
			{
				_infoDisplayStatus = value;
				NotifyOfPropertyChange(() => InfoDisplayStatus);
			}
		}
	}

	public double WindSpeed
	{
		get { return _weatherInfo.WindSpeed ?? 0.0; }
	}

	public double WindDirection
	{
		get { return _weatherInfo.WindDirection ?? 0.0; }
	}

	public ICommand SettingsCommand
	{
		get { return _settingsCommand; }
	}

	public MainViewModel(IWpfContext wpfContext, IWindowManager windowManager,
		IServiceProvider serviceProvider, IEventAggregator eventAggregator, 
		IUserSettings userSettings, ILogger<MainViewModel> logger)
	{
		_wpfContext = wpfContext ?? throw new ArgumentNullException(nameof(wpfContext));
		_windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
		_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
		_userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
		_logger = logger;

		_settingsCommand = new RelayCommand(SettingsButton_Clicked);

		_weatherInfo = EmptyWeatherInfo.Instance;

		_settings = new AppSettings { RefreshIntervalSeconds = 10 };
		_eventAggregator.SubscribeOnPublishedThread(this);
	}

	protected override async void OnViewLoaded(object view)
	{
		base.OnViewLoaded(view);
		try
		{
			await _userSettings.Load();
			if(_userSettings != null && _userSettings.SelectedCity != null)
			{
				DisplayedCity = _userSettings.SelectedCity;
				ResetInfoDisplay();
			}
			RaiseLocationChanged();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed loading user settings");
		}		
	}

	private void RaiseLocationChanged()
	{
		if(_userSettings != null && _userSettings.SelectedCountry != null && _userSettings.SelectedCity != null)
			_eventAggregator.PublishOnCurrentThreadAsync(
				new LocationChanged(new Location { Country = _userSettings.SelectedCountry, City = _userSettings.SelectedCity}));
	}
/*
	public void Init()
	{
		if (!String.IsNullOrEmpty(_settings.SelectedCity))
			_displayedCity = _settings.SelectedCity;

		bool isLocationDataAvailable = _yrWeatherDataStorage.IsLocationDataAvailable();
		IsDownloadProgressLayerVisible = !isLocationDataAvailable;
		if(!isLocationDataAvailable)
		{
			DownloadLocationData();
		}
		else
		{
			_globalDataContainer.YrLocationData = _yrWeatherDataStorage.GetLocationData();
		}

		if (!String.IsNullOrEmpty(_settings.SelectedCountry) && _globalDataContainer.YrLocationData.Countries.ContainsKey(_settings.SelectedCountry))
		{
			DisplayedCountry = _globalDataContainer.YrLocationData.Countries[_settings.SelectedCountry];
			if (!String.IsNullOrEmpty(_settings.SelectedCity))
				_displayedCity = _settings.SelectedCity;
			GetWeather(true);
			ResetInfoDisplay();
		}
		else
		{
			IsSettingsLayerVisible = true;
			_eventAggregator.PublishOnUIThreadAsync(new SettingsRequestedEventData());
		}

		_weatherInfoUpdateTimer.Start();
	}
*/
	private void SettingsButton_Clicked()
	{
		IsSettingsLayerVisible = true;
		//_windowManager.ShowWindowAsync((UserSettingsViewModel)_serviceProvider.GetService(typeof(UserSettingsViewModel))!);
		_eventAggregator.PublishOnUIThreadAsync(new SettingsRequestedEventData());
	}

	private void SetInfoDisplayError(string message)
	{
		InfoDisplayStatus = ViewModels.InfoDisplayStatus.Error;
		InfoDisplayString = message;
	}

	private void ResetInfoDisplay()
	{
		InfoDisplayStatus = ViewModels.InfoDisplayStatus.Normal;
		InfoDisplayString = DisplayedCity ?? DEFAULT_DISPLAYED_CITY;
	}

	private string? GetWeatherServiceExceptionDisplayedMessage(WeatherServiceException wex)
	{
		return DisplayedErrors.ResourceManager.GetString(wex.Reason.ToString());
	}

	public async Task HandleAsync(SettingsOkayedEventData message, CancellationToken cancellationToken)
	{
		_displayedCity = message.Location.City;
		ResetInfoDisplay();
		IsSettingsLayerVisible = false;
		WeatherInfo = EmptyWeatherInfo.Instance;
		try
		{
			await _userSettings.Save();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed saving user settings");
		}
		
		RaiseLocationChanged();
	}

	public Task HandleAsync(SettingsCancelledEventData message, CancellationToken cancellationToken)
	{
		IsSettingsLayerVisible = false;
		return Task.CompletedTask;
	}

    public Task HandleAsync(IWeatherUpdated message, CancellationToken cancellationToken)
    {
        WeatherInfo = message.Weather;
		return Task.CompletedTask;
    }

    public Task HandleAsync(ILocationChanged message, CancellationToken cancellationToken)
    {
        DisplayedCity = message.Location.City;
		return Task.CompletedTask;
    }

    private class EmptyWeatherInfo : IWeatherInfo
	{
		public double? Temperature => null;

		public double? WindDirection => null;

		public double? WindSpeed => null;

		public static EmptyWeatherInfo Instance { get; private set; } = new EmptyWeatherInfo();
	}	

    private class Location : ILocation
    {
        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
    }

    private class LocationChanged(ILocation _location) : ILocationChanged
    {
        public ILocation Location => _location;
    }
}