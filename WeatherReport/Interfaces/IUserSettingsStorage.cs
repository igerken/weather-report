namespace WeatherReport.WinApp.Interfaces;

public interface IUserSettingsStorage
{
    Task<IUserSettings?> Load();

    Task Save(IUserSettings settings);
}
