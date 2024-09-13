using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using WeatherReport.App.Interfaces;
using WeatherReport.Core;
using WeatherReport.Data;
using WeatherReport.Events;
using WeatherReport.MVVM;

namespace WeatherReport.WinApp.ViewModels;

public class WeatherViewModel : PropertyChangedBase, 
	IHandle<SettingsOkayedEventData>,
	IHandle<SettingsCancelledEventData>
{
	public const string PROP_WEATHER_INFO = "WeatherInfo";

	private const string DEFAULT_DISPLAYED_CITY = "----";        

	private readonly IWeatherService _weatherService;
	//private readonly IYrWeatherDataStorage _yrWeatherDataStorage;
	private readonly IEventAggregator _eventAggregator;
	//private readonly IGlobalDataContainer _globalDataContainer;
	private readonly ILogger _logger;

	private readonly Interfaces.IUserSettings _userSettings;


	private readonly Timer _serviceRefreshTimer;
	private readonly DispatcherTimer _weatherInfoUpdateTimer;	


	private bool _isSettingsLayerVisible = false;
	private bool _isDownloadProgressLayerVisible;

	private RelayCommand _settingsCommand;

	private IWeatherInfo _weatherInfo;
	private YrCountryData _displayedCountry;
	private string _displayedCity;
	private string _infoDisplayString = DEFAULT_DISPLAYED_CITY;

	private bool _isNewWeatherInfoRetrieved;
	private IWeatherInfo? _newWeatherInfo = null;
	private string? _newWeatherRetrievalError = null;

	private InfoDisplayStatus _infoDisplayStatus = InfoDisplayStatus.Normal;

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
	
	public YrCountryData DisplayedCountry
	{
		get { return _displayedCountry; }
		set
		{
			if (_displayedCountry != value)
			{
				_displayedCountry = value;
				NotifyOfPropertyChange(() => DisplayedCountry);
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
				String.Format("{0,4:0.0}°C", _weatherInfo.Temperature.Value) : "--.-°C";
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

	public WeatherViewModel(IWeatherService weatherService, IEventAggregator eventAggregator, //IGlobalDataContainer globalDataContainer, 
		IUserSettings userSettings, ILogger logger)
	{
		_weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
		_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
		_logger = logger;
/*
		_yrWeatherDataStorage = yrWeatherDataStorage;
		_globalDataContainer = globalDataContainer;
		_settings = settings;
*/
		_settingsCommand = new RelayCommand(SettingsButton_Clicked);

		_weatherInfo = EmptyWeatherInfo.Instance;

		int refreshMs = 1000 * settings.RefreshIntervalSeconds;
		_serviceRefreshTimer = new Timer(callback => GetWeather(false), null, refreshMs, refreshMs);

		_weatherInfoUpdateTimer = new DispatcherTimer();
		_weatherInfoUpdateTimer.Interval = TimeSpan.FromMilliseconds(5000);// (500 * settings.RefreshIntervalSeconds);
		_weatherInfoUpdateTimer.Tick += new EventHandler((s, e) =>
		{
			if(_isNewWeatherInfoRetrieved)
			{
				WeatherInfo = _newWeatherInfo;
				_isNewWeatherInfoRetrieved = false;
			}
		});

		_eventAggregator.SubscribeOnPublishedThread(this);
	}

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
			_eventAggregator.PublishOnUIThread(new SettingsRequestedEventData());
		}

		_weatherInfoUpdateTimer.Start();
	}

	private void SettingsButton_Clicked()
	{
		IsSettingsLayerVisible = true;
		_eventAggregator.PublishOnUIThread(new SettingsRequestedEventData());
	}

	private async Task GetWeather(bool updateImmediately)
	{
		if (DisplayedCountry != null && DisplayedCity != null)
		{
			Progress<DownloadProgressData> progress = new Progress<DownloadProgressData>(
				progressData => _eventAggregator.PublishOnUIThreadAsync(new DownloadProgressChangedEventData(progressData))
			);
			try
			{
				YrCityData city = DisplayedCountry.Cities.FirstOrDefault(c => c.Name == DisplayedCity);
				if (city != null)
				{
					IWeatherInfo weatherInfo = await _weatherService.GetWeather(city.DataUrl, progress).ConfigureAwait(false);
					if (updateImmediately)
					{
						WeatherInfo = weatherInfo;
						_isNewWeatherInfoRetrieved = false;
					}
					else
					{
						_newWeatherInfo = weatherInfo;
						_isNewWeatherInfoRetrieved = true;
					}
				}
			}
			catch (WeatherServiceException wex)
			{
				_newWeatherRetrievalError = GetWeatherServiceExceptionDisplayedMessage(wex);
				_logger.LogError(wex, "GetWeather: Failed to retrieve weather info");
			}                
		}
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

	public async void DownloadLocationData()
	{
		Progress<DownloadProgressData> progress = new Progress<DownloadProgressData>(
			progressData => _eventAggregator.PublishOnUIThread(new DownloadProgressChangedEventData(progressData))
		);
		YrLocationDataContainer locationData = await _weatherService.GetLocationData(progress).ConfigureAwait(false);
		_globalDataContainer.YrLocationData = locationData;
		IsDownloadProgressLayerVisible = false;
		IsSettingsLayerVisible = true;
		_yrWeatherDataStorage.StoreLocationData(_globalDataContainer.YrLocationData);
		_eventAggregator.PublishOnUIThread(new SettingsRequestedEventData());
	}

	public async Task HandleAsync(SettingsOkayedEventData message, CancellationToken cancellationToken)
	{
		if(_globalDataContainer.YrLocationData.Countries.ContainsKey(message.CountryCode))
			DisplayedCountry = _globalDataContainer.YrLocationData.Countries[message.CountryCode];
		_displayedCity = message.City;
		ResetInfoDisplay();
		IsSettingsLayerVisible = false;
		WeatherInfo = EmptyWeatherInfo.Instance;
		IsDownloadProgressLayerVisible = true;
		await _eventAggregator.PublishOnUIThreadAsync(new DownloadRequestedEventData());
		await GetWeather(true);
		IsDownloadProgressLayerVisible = false;
	}

	public Task HandleAsync(SettingsCancelledEventData message, CancellationToken cancellationToken)
	{
		IsSettingsLayerVisible = false;
		return Task.CompletedTask;
	}

	private class EmptyWeatherInfo : IWeatherInfo
	{
		public double? Temperature => null;

		public double? WindDirection => null;

		public double? WindSpeed => null;

		public static EmptyWeatherInfo Instance { get; private set; } = new EmptyWeatherInfo();
	}
}