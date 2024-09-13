using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using Caliburn.Micro;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

using WeatherReport.WinApp.Data;
using WeatherReport.WinApp.Interfaces;
using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp;

public class AppBootstrapper : BootstrapperBase
{
    public IServiceProvider? ServiceProvider { get; private set; }

	public IConfiguration? Configuration { get; private set; }

	public AppBootstrapper()
	{
		Initialize();

		var baseLocateTypeForModelType = ViewLocator.LocateTypeForModelType;
		ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
		{
			if (modelType.Equals(typeof(WeatherViewModel)))
				return typeof(MainWindow);
			return baseLocateTypeForModelType(modelType, displayLocation, context);
		};

		var baseLocateTypeForViewType = ViewModelLocator.LocateTypeForViewType;
		ViewModelLocator.LocateTypeForViewType = (viewType, searchForInterface) =>
		{
			if (viewType.Equals(typeof(UserSettingsControl)))
				return typeof(UserSettingsViewModel);
			return baseLocateTypeForViewType(viewType, searchForInterface);
		};
	}

	protected override void Configure()
	{
		var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        var serviceCollection = new ServiceCollection();

		serviceCollection.AddSingleton<IWindowManager, WindowManager>();
		serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();

		//_container.Singleton<IGlobalDataContainer, GlobalDataContainer>();
		//_container.Singleton<IYrWeatherService, YrWeatherService>();
		//_container.Singleton<IYrWeatherDataStorage, YrWeatherDataStorage>();
		_container.RegisterInstance(typeof(log4net.ILog), null, log4net.LogManager.GetLogger(typeof(WeatherViewModel)));
		_container.RegisterInstance(typeof(IWeatherSettings), null, Settings.Default);
		//serviceCollection.AddSerilog()
		serviceCollection.AddScoped<WeatherViewModel>();
		serviceCollection.AddScoped<DownloadProgressViewModel>();
		serviceCollection.AddScoped<UserSettingsViewModel>();

        ServiceProvider = serviceCollection.BuildServiceProvider();		
	}

	protected override object GetInstance(Type serviceType, string key)
	{
		var instance = _container.GetInstance(serviceType, key);

		if (instance == null && key== "WeatherReport.ViewModels.UserSettingsViewModel")
			instance = _container.GetInstance(typeof(UserSettingsViewModel), null);

		if (instance == null)
			throw new InvalidOperationException(String.Format("Could not resolve type {0}", serviceType));

		WeatherViewModel? weatherInstance = instance as WeatherViewModel;
		if (weatherInstance != null)
			weatherInstance.Init();

		return instance;
	}

	protected override IEnumerable<object> GetAllInstances(Type serviceType)
	{
		return _container.GetAllInstances(serviceType);
	}

	protected override void BuildUp(object instance)
	{
		_container.BuildUp(instance);
	}

	protected override async void OnStartup(object sender, StartupEventArgs e)
	{
		await DisplayRootViewForAsync<WeatherViewModel>();
		WeatherViewModel weatherViewModel = _container.GetInstance<WeatherViewModel>(null);
		if (weatherViewModel != null)
			weatherViewModel.Init();
	}
}