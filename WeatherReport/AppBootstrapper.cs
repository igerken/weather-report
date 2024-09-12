using System;
using System.Collections.Generic;
using System.Windows;

using Caliburn.Micro;
using WeatherReport.DataModel;
using WeatherReport.Interfaces;
using WeatherReport.Services;
using WeatherReport.ViewModels;

namespace WeatherReport
{
	public class AppBootstrapper : BootstrapperBase
	{
		SimpleContainer _container = new SimpleContainer();
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
			_container.Singleton<IWindowManager, WindowManager>();
			_container.Singleton<IEventAggregator, EventAggregator>();

			_container.Singleton<IGlobalDataContainer, GlobalDataContainer>();
			_container.Singleton<IYrWeatherService, YrWeatherService>();
			_container.Singleton<IYrWeatherDataStorage, YrWeatherDataStorage>();
			_container.RegisterInstance(typeof(log4net.ILog), null, log4net.LogManager.GetLogger(typeof(WeatherViewModel)));
			_container.RegisterInstance(typeof(IWeatherSettings), null, Settings.Default);
			_container.PerRequest<WeatherViewModel>();
			_container.PerRequest<DownloadProgressViewModel>();
			_container.PerRequest<UserSettingsViewModel>();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			var instance = _container.GetInstance(serviceType, key);

            if (instance == null && key== "WeatherReport.ViewModels.UserSettingsViewModel")
                instance = _container.GetInstance(typeof(UserSettingsViewModel), null);

            if (instance == null)
				throw new InvalidOperationException(String.Format("Could not resolve type {0}", serviceType));

			//WeatherViewModel weatherInstance = instance as WeatherViewModel;
			//if (weatherInstance != null)
			//	weatherInstance.Init();

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

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
            DisplayRootViewFor<WeatherViewModel>();
            WeatherViewModel weatherViewModel = _container.GetInstance<WeatherViewModel>(null);
            if (weatherViewModel != null)
                weatherViewModel.Init();

        }
    }
}
