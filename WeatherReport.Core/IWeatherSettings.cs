using System;
using System.Collections.Specialized;

namespace WeatherReport.Interfaces
{
    public interface IWeatherSettings
    {
        StringCollection Countries { get; }
        int RefreshIntervalSeconds { get; }
        string SelectedCountry { get; set; }
        string SelectedCity { get; set; }

        void Save();
    }
}
