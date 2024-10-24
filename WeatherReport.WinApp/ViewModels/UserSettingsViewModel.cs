using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Caliburn.Micro;
using Microsoft.Extensions.Options;
using WeatherReport.Core.Settings;
using WeatherReport.WinApp.Events;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.WinApp.ViewModels
{
    public class UserSettingsViewModel : PropertyChangedBase, IHandle<SettingsRequested>
	{
		private readonly IEventAggregator _eventAggregator;
        private readonly IOptions<List<LocationSettings>> _locationSettingsOptions;
        private readonly IUserSettings _userSettings;

        private Lazy<string[]> _countriesLazy;

        private string? _initialCountry;
        private string? _initialCity;
        
        private string? _selectedCountry;
        private string? _selectedCity;

        public string[] Countries
		{
            get { return _countriesLazy.Value; }
        }

        public ObservableCollection<string> Cities { get; }

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

        public string? SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
					NotifyOfPropertyChange(() => SelectedCity);
					NotifyOfPropertyChange(() => CanOkSettings);                    
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool CanOkSettings => !string.IsNullOrEmpty(SelectedCity);

        public UserSettingsViewModel(IUserSettings userSettings, IEventAggregator eventAggregator, 
            IOptions<List<LocationSettings>> locationSettingsOptions)
        {
            _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
		    _locationSettingsOptions = locationSettingsOptions ?? throw new ArgumentNullException(nameof(locationSettingsOptions));

            _countriesLazy = new Lazy<string[]>(() => 
                _locationSettingsOptions.Value.Select(s => s.Country).Distinct().OrderBy(c => c).ToArray());
            Cities = new ObservableCollection<string>();

            _userSettings.PropertyChanged += HandleUserSettingsChanged;

			_eventAggregator.SubscribeOnPublishedThread(this);
		}

        public Task HandleAsync(SettingsRequested message, CancellationToken cancellationToken)
		{
			if (!string.IsNullOrEmpty(_userSettings.SelectedCountry))
			{
                _initialCountry = _userSettings.SelectedCountry;
                SelectedCountry = _userSettings.SelectedCountry;
			}

			if (!string.IsNullOrEmpty(_userSettings.SelectedCity))
			{
				_initialCity = _userSettings.SelectedCity;
				SelectedCity = _userSettings.SelectedCity;
			}
            
            return Task.CompletedTask;
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

        public void OkSettings()
        {
            _userSettings.SelectedCountry = SelectedCountry;
            _userSettings.SelectedCity = SelectedCity;

			_eventAggregator.PublishOnUIThreadAsync(new SettingsOkayedEventData(SelectedCountry!, SelectedCity!));
        }

        public void CancelSettings()
        {
            _selectedCountry = _initialCountry;
            _selectedCity = _initialCity;

            UpdateCityList();
			NotifyOfPropertyChange(() => SelectedCountry);
			NotifyOfPropertyChange(() => SelectedCity);

			_eventAggregator.PublishOnUIThreadAsync(new SettingsCancelled());
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
        }
    }
}