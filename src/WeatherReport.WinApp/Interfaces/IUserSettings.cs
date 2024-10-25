using System.ComponentModel;

namespace WeatherReport.WinApp.Interfaces;

public interface IUserSettings
{    
    string? SelectedCountry { get; set; }
    string? SelectedCity { get; set; }

    event PropertyChangedEventHandler PropertyChanged;

    Task Load();

    Task Save();
}