using System.Collections.Generic;

using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

using WeatherReport.Core.Settings;
using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.UserSettingsViewModelTests;

public class SelectedCountry
{
    private const string COUNTRY_CZECHIA = "CZ";
    private const string COUNTRY_DENMARK = "DK";
    private const string CITY_BRNO = "Brno";
    private const string CITY_PRAGUE = "Praha";
    private const string CITY_COPENHAGEN = "København";
    
    [Theory, AutoFakeData]
    public void Given_SelectedCountryChanged__Then_CitiesUpdated(
        [Frozen] Mock<IOptions<List<LocationSettings>>> locationSettingsOptionsMock,
        UserSettingsViewModel sut)
    {
        locationSettingsOptionsMock.Setup(lso => lso.Value).Returns(GetDefaultLocationSettings());

        // --- Act
        sut.SelectedCountry = COUNTRY_DENMARK;

        // --- Assert            
        Assert.Contains(CITY_COPENHAGEN, sut.Cities);
    }

    private List<LocationSettings> GetDefaultLocationSettings()
    {
        return [
            new LocationSettings { Country = COUNTRY_CZECHIA, City = CITY_BRNO },
            new LocationSettings { Country = COUNTRY_CZECHIA, City = CITY_PRAGUE },
            new LocationSettings { Country = COUNTRY_DENMARK, City = CITY_COPENHAGEN }
        ];
    }
}
