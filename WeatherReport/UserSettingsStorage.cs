using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using WeatherReport.Data;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.WinApp;

public class UserSettingsStorage : IUserSettingsStorage
{
    private const string SettingsFileName = "WeatherReportSettings.json";

    public async Task Save(IUserSettings settings)
    {
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Create, isoStore))
        {
            await JsonSerializer.SerializeAsync(isoStream, settings);
        }
    }

    public async Task<IUserSettings?> Load()
    {
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        if (isoStore.FileExists(SettingsFileName))
        {
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Open, isoStore))
            {
                return await JsonSerializer.DeserializeAsync<UserSettings>(isoStream);
            }
        }

        return null;
    }
}
