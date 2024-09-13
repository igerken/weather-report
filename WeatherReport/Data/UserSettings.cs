using System;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.Data;

public class UserSettings : IUserSettings
{
    public string? SelectedCountry { get; set; }
    public string? SelectedCity { get; set; }

    public Task Save()
    {
        throw new NotImplementedException();
    }
}
