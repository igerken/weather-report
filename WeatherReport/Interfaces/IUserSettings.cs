namespace WeatherReport.WinApp.Interfaces;

public interface IUserSettings
{    
    string SelectedCountry { get; set; }
    string SelectedCity { get; set; }

    Task Save();
}