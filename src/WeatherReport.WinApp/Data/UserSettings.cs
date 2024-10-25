using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using Caliburn.Micro;
using WeatherReport.WinApp.Interfaces;

namespace WeatherReport.Data;

public class UserSettings : PropertyChangedBase, IUserSettings
{
    private const string SettingsFileName = "WeatherReportSettings.json";
        
    private string? _selectedCountry;
    private string? _selectedCity;

    public string? SelectedCountry
    {
        get { return _selectedCountry; }
        set
        {
            if (_selectedCountry != value)
            {
                _selectedCountry = value;
                NotifyOfPropertyChange(() => SelectedCountry);
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
            }
        }
    }

    public async Task Save()
    {
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Create, isoStore))
        {
            await JsonSerializer.SerializeAsync(isoStream, this);
        }
    }

    public async Task Load()
    {
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        if (isoStore.FileExists(SettingsFileName))
        {
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Open, isoStore))
            {
                UserSettings? loaded = await JsonSerializer.DeserializeAsync<UserSettings>(isoStream);
                if(loaded != null)
                {
                    SelectedCountry = loaded.SelectedCountry;
                    SelectedCity = loaded.SelectedCity;
                }
            }
        }
    }
}
