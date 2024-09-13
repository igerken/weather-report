using System;
using System.Collections.Generic;
using Caliburn.Micro;

using WeatherReport.WinApp.Data;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.WinApp.ViewModels;

public class ViewModelLocator
{
	//private static readonly ViewModelLocator _instance = new ViewModelLocator();

	private readonly WeatherViewModel _weatherViewModel;
	private readonly UserSettingsViewModel _userSettingsViewModel;
	private readonly DownloadProgressViewModel _downloadProgressViewModel;

	private readonly GlobalDataContainer _globalDataContainer;

	private static readonly EventAggregator _eventAggregator = new EventAggregator();

	//public static ViewModelLocator Current
	//      {
	//          get { return _instance; }
	//      }

	public WeatherViewModel WeatherViewModel
	{
		get { return _weatherViewModel; }
	}
	public UserSettingsViewModel UserSettingsViewModel
	{
		get { return _userSettingsViewModel; }
	}
	public DownloadProgressViewModel DownloadProgressViewModel
	{
		get { return _downloadProgressViewModel; }
	}

	public ViewModelLocator()
	{
		_globalDataContainer = new GlobalDataContainer();
		//_eventAggregator = new EventAggregator();
		IYrWeatherService yrWeatherService = new YrWeatherService();
		IYrWeatherDataStorage yrWeatherDataStorage = new YrWeatherDataStorage();

		_downloadProgressViewModel = new DownloadProgressViewModel(yrWeatherService, _globalDataContainer, _eventAggregator);

		_userSettingsViewModel = new UserSettingsViewModel(_globalDataContainer, _eventAggregator,
			log4net.LogManager.GetLogger(typeof(UserSettingsViewModel)), Settings.Default);

		_weatherViewModel = new WeatherViewModel(yrWeatherService, yrWeatherDataStorage, _eventAggregator, _globalDataContainer,
			log4net.LogManager.GetLogger(typeof(WeatherViewModel)), Settings.Default);
		_weatherViewModel.Init();            
	}
}