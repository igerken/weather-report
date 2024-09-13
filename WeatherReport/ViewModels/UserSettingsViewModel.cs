using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using WeatherReport.WinApp.Data;
using WeatherReport.WinApp.Events;
using WeatherReport.Interfaces;
using WeatherReport.MVVM;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.WinApp.ViewModels
{
    public class UserSettingsViewModel : PropertyChangedBase, IHandle<SettingsRequestedEventData>
	{
        private const string DEFAULT_CITY_LIST_RETRIEVAL_ERROR_MESSAGE = "";

		private readonly IEventAggregator _eventAggregator;
		private readonly ILogger _logger;
        private IUserSettings _userSettings;

        private RelayCommand _settingsOkCommand;
        private RelayCommand _settingsCancelCommand;
/*
        private List<YrCountryData> _countries;
        private YrCountryData _initialCountry;
        private YrCountryData _selectedCountry;
*/
        private ObservableCollection<string> _cities;
        //private string _initialCity;
        private string? _selectedCity;

        private bool _isCityListAvailable = true;
        private string _cityListRetrievalErrorMessage = DEFAULT_CITY_LIST_RETRIEVAL_ERROR_MESSAGE;

        private object _cityListLock = new object();

        public ICommand SettingsOkCommand
        {
            get { return _settingsOkCommand; }
        }
        public ICommand SettingsCancelCommand
        {
            get { return _settingsCancelCommand; }
        }
/*
        public List<YrCountryData> Countries
		{
            get { return _countries; }
        }

        public YrCountryData SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                if (_selectedCountry != value)
                {
					_selectedCountry = value;
					NotifyOfPropertyChange(() => SelectedCountry);
                    UpdateCityList();
                }
            }
        }
*/
        public ObservableCollection<string> Cities
        {
            get { return _cities; }
        }

        public string? SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
					NotifyOfPropertyChange(() => SelectedCity);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsCityListAvailable
        {
            get { return _isCityListAvailable; }
            set
            {
                if (_isCityListAvailable != value)
                {
                    _isCityListAvailable = value;
					NotifyOfPropertyChange(() => IsCityListAvailable);
					NotifyOfPropertyChange(() => IsCityListUnavailable);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsCityListUnavailable
        {
            get { return !IsCityListAvailable; }
        }

        public string CityListRetrievalErrorMessage
        {
            get { return _cityListRetrievalErrorMessage; }
            set
            {
                if (_cityListRetrievalErrorMessage != value)
                {
                    _cityListRetrievalErrorMessage = value;
					NotifyOfPropertyChange(() => CityListRetrievalErrorMessage);
                }
            }
        }

        public UserSettingsViewModel(IUserSettings userSettings, IEventAggregator eventAggregator, ILogger logger)
        {
            _userSettings = userSettings;
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
			_logger = logger;

            _settingsOkCommand = new RelayCommand(SettingsOkButton_Clicked, 
                () => (IsCityListAvailable && !String.IsNullOrEmpty(SelectedCity)));
            _settingsCancelCommand = new RelayCommand(SettingsCancelButton_Clicked);

			//_countries = new List<YrCountryData>();
            _cities = new ObservableCollection<string>();

			_eventAggregator.SubscribeOnPublishedThread(this);
		}

        private void SettingsOkButton_Clicked()
        {/*
            _userSettings.SelectedCountry = SelectedCountry.CountryCode;
            _userSettings.SelectedCity = SelectedCity;
            _userSettings.Save();

            _initialCountry = SelectedCountry;
            _initialCity = SelectedCity;

			_eventAggregator.PublishOnUIThread(new SettingsOkayedEventData(SelectedCountry.CountryCode, SelectedCity));*/
        }

        private void SettingsCancelButton_Clicked()
        {/*
            _selectedCountry = _initialCountry;
            _selectedCity = _initialCity;

            UpdateCityList(false);
			NotifyOfPropertyChange(() => SelectedCountry);
			NotifyOfPropertyChange(() => SelectedCity);

			_eventAggregator.PublishOnUIThread(new SettingsCancelledEventData());*/
		}

        private void UpdateCityList()
        {
            UpdateCityList(true);
        }

        private void UpdateCityList(bool resetSelectedCity)
        {/*
			if(_globalDataContainer.YrLocationData.Countries.ContainsKey(SelectedCountry.CountryCode))
			{
				YrCountryData country = _globalDataContainer.YrLocationData.Countries[SelectedCountry.CountryCode];
				IEnumerable<string> cities = country.Cities.Select(aCity => aCity.Name);
				string oldSelectedCity = SelectedCity;
				lock (_cityListLock)
				{
					_cities.Clear();
					foreach (string city in cities.OrderBy(c => c))
						_cities.Add(city);
				}
				if (resetSelectedCity)
					SelectedCity = null;
				else
				{
					if (_cities.Contains(oldSelectedCity))
						SelectedCity = oldSelectedCity;
				}
				IsCityListAvailable = true;
				CityListRetrievalErrorMessage = DEFAULT_CITY_LIST_RETRIEVAL_ERROR_MESSAGE;
			}	*/		
        }

        public Task HandleAsync(SettingsRequestedEventData message, CancellationToken cancellationToken)
		{/*
			_countries = _globalDataContainer.YrLocationData.Countries.Values.ToList();
			NotifyOfPropertyChange(() => Countries);

			if (!String.IsNullOrEmpty(_settings.SelectedCountry))
			{
				YrCountryData selected = _countries.FirstOrDefault(c => c.CountryCode.Equals(
					_settings.SelectedCountry, StringComparison.InvariantCultureIgnoreCase));
				if (selected != null)
				{
					_initialCountry = selected;
					SelectedCountry = selected;
					UpdateCityList(false);
				}
			}

			if (!String.IsNullOrEmpty(_settings.SelectedCity))
			{
				_initialCity = _settings.SelectedCity;
				SelectedCity = _settings.SelectedCity;
			}*/
            return Task.CompletedTask;
		}
    }
}
