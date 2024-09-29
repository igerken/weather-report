using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Caliburn.Micro;
using Microsoft.Extensions.Options;
using WeatherReport.Core.Settings;
using WeatherReport.MVVM;
using WeatherReport.WinApp.Events;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.WinApp.ViewModels
{
    public class UserSettingsViewModel : PropertyChangedBase, IHandle<SettingsRequestedEventData>
	{
		private readonly IEventAggregator _eventAggregator;
        private readonly IOptions<List<LocationSettings>> _locationSettingsOptions;
        private readonly IUserSettings _userSettings;

        private List<string> _countries;

        private string? _initialCountry;
        private string? _initialCity;
        
        private string? _selectedCountry;
        private string? _selectedCity;

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

        public UserSettingsViewModel(IUserSettings userSettings, IEventAggregator eventAggregator, 
            IOptions<List<LocationSettings>> locationSettingsOptions)
        {
            _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
			_eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
		    _locationSettingsOptions = locationSettingsOptions ?? throw new ArgumentNullException(nameof(locationSettingsOptions));

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
        }

        public Task HandleAsync(SettingsRequestedEventData message, CancellationToken cancellationToken)
		{
			if (!string.IsNullOrEmpty(_userSettings.SelectedCountry))
			{
                _initialCountry = _userSettings.SelectedCountry;
                SelectedCountry = _userSettings.SelectedCountry;
                UpdateCityList();
			}

			if (!string.IsNullOrEmpty(_userSettings.SelectedCity))
			{
				_initialCity = _userSettings.SelectedCity;
				SelectedCity = _userSettings.SelectedCity;
			}
            return Task.CompletedTask;
		}
    }
}