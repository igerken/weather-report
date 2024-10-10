using System;
using System.Collections.Specialized;
using WeatherReport.Interfaces;

namespace WeatherReport.UnitTests
{
    public class DummyWeatherSettings : IWeatherSettings
    {
        public StringCollection Countries { get; set; }
        public int RefreshIntervalSeconds { get; set; }

        public string SelectedCountry { get; set; }
        public string SelectedCity { get; set; }

        public void Save()
        {
        }

        public DummyWeatherSettings()
            : this(new StringCollection(), null, null, 0)
        {
        }

        public DummyWeatherSettings(StringCollection countries)
            : this(countries, null, null, 0)
        {
        }

        public DummyWeatherSettings(StringCollection countries, string selectedCountry, string selectedCity)
            : this(countries, selectedCountry, selectedCity, 0)
        {
        }

        public DummyWeatherSettings(StringCollection countries, 
            string selectedCountry, string selectedCity, int refreshIntervalSeconds)
        {
            Countries = countries;
            SelectedCountry = selectedCountry;
            SelectedCity = selectedCity;
            RefreshIntervalSeconds = refreshIntervalSeconds;
        }
    }
}
