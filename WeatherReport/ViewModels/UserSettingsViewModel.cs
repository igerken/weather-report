using System.Collections.ObjectModel;
using System.Windows.Input;

using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using WeatherReport.WinApp.Data;
using WeatherReport.WinApp.Events;
using WeatherReport.Interfaces;
using WeatherReport.MVVM;
using WeatherReport.WinApp.Interfaces;
using Microsoft.Extensions.Options;
using WeatherReport.Core.Settings;
using System.ComponentModel;

namespace WeatherReport.WinApp.ViewModels
{
    public class UserSettingsViewModel : PropertyChangedBase, IHandle<SettingsRequestedEventData>
	{
        private const string DEFAULT_CITY_LIST_RETRIEVAL_ERROR_MESSAGE = "";

		private readonly IEventAggregator _eventAggregator;

        private readonly IOptions<List<LocationSettings>> _locationSettingsOptions;
		private readonly ILogger<UserSettingsViewModel> _logger;
        private readonly IUserSettings _userSettings;

        private List<string> _countries;

        private string? _initialCountry;
        private string? _initialCity;
        
        private string? _selectedCountry;
        private string? _selectedCity;
        private string _cityListRetrievalErrorMessage = DEFAULT_CITY_LIST_RETRIEVAL_ERROR_MESSAGE;

        public ICommand SettingsOkCommand { get; }

        public ICommand SettingsCancelCommand { get; }

        public List<string> Countries
		{
            get { return _countries; }
        }

        public string? SelectedCountry
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

        public ObservableCollection<string> Cities { get; }

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

        public UserSettingsViewModel(IUserSettings userSettings, IEventAggregator eventAggregator, 
            IOptions<List<LocationSettings>> locationSettingsOptions, ILogger<UserSettingsViewModel> logger)
        {
            _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
		    _locationSettingsOptions = locationSettingsOptions ?? throw new ArgumentNullException(nameof(locationSettingsOptions));
			_logger = logger;

            SettingsOkCommand = new RelayCommand(SettingsOkButton_Clicked, 
                () => !string.IsNullOrEmpty(SelectedCity));
            SettingsCancelCommand = new RelayCommand(SettingsCancelButton_Clicked);

			_countries = _locationSettingsOptions.Value.Select(s => s.Country).Distinct().OrderBy(c => c).ToList();
            Cities = new ObservableCollection<string>();

            _initialCountry = userSettings.SelectedCountry;
            SelectedCountry = userSettings.SelectedCountry;

            _initialCity = userSettings.SelectedCity;
            SelectedCity = userSettings.SelectedCity;

            _userSettings.PropertyChanged += HandleUserSettingsChanged;

			_eventAggregator.SubscribeOnPublishedThread(this);
		}

        private void HandleUserSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(SelectedCountry):
                    SelectedCountry = _userSettings.SelectedCountry;
                    break;
                case nameof(SelectedCity):
                    SelectedCity = _userSettings.SelectedCity;
                    break;
            }
        }

        private void SettingsOkButton_Clicked()
        {
            _userSettings.SelectedCountry = SelectedCountry;
            _userSettings.SelectedCity = SelectedCity;

			_eventAggregator.PublishOnUIThreadAsync(new SettingsOkayedEventData(SelectedCountry!, SelectedCity!));
        }

        private void SettingsCancelButton_Clicked()
        {
            _selectedCountry = _initialCountry;
            _selectedCity = _initialCity;

            UpdateCityList();
			NotifyOfPropertyChange(() => SelectedCountry);
			NotifyOfPropertyChange(() => SelectedCity);

			_eventAggregator.PublishOnUIThreadAsync(new SettingsCancelledEventData());
		}

        private void UpdateCityList()
        {
            Cities.Clear();
            _locationSettingsOptions.Value
                .Where(opt => opt.Country == SelectedCountry)
                .Select(opt => opt.City)
                .OrderBy(c => c)
                .ToList()
                .ForEach(Cities.Add);
			
            /*
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
