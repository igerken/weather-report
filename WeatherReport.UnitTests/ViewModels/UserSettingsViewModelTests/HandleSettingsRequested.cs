using System.Collections.Generic;
using System.Threading;

using AutoFixture.Xunit2;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

using WeatherReport.Core.Settings;
using WeatherReport.WinApp.Events;
using WeatherReport.WinApp.Interfaces;
using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.UserSettingsViewModelTests
{
    public class HandleSettingsRequested
    {
        private const string COUNTRY_CZECHIA = "CZ";
        private const string COUNTRY_DENMARK = "DK";
        private const string CITY_BRNO = "Brno";
        private const string CITY_PRAGUE = "Praha";
        private const string CITY_COPENHAGEN = "København";

        [Theory, AutoFakeData]
        public void Given_SelectedCountryCityUserSettingsAvailable__Then_CountryCityValuesSet_CityListPopulated(
            [Frozen] Mock<IUserSettings> userSettingsMock,
            [Frozen] Mock<IOptions<List<LocationSettings>>> locationSettingsOptionsMock,
            UserSettingsViewModel sut)
        {
            userSettingsMock.Setup(uset => uset.SelectedCountry).Returns(COUNTRY_CZECHIA);
            userSettingsMock.Setup(uset => uset.SelectedCity).Returns(CITY_BRNO);

            locationSettingsOptionsMock.Setup(lso => lso.Value).Returns(GetDefaultLocationSettings());
            
            // --- Act
            sut.HandleAsync(new SettingsRequestedEventData(), CancellationToken.None);

            // --- Assert
            Assert.Equal(COUNTRY_CZECHIA, sut.SelectedCountry);
            Assert.Equal(CITY_BRNO, sut.SelectedCity);
            Assert.Contains(CITY_BRNO, sut.Cities);
            Assert.Contains(CITY_PRAGUE, sut.Cities);
            Assert.DoesNotContain(CITY_COPENHAGEN, sut.Cities);
        }

        [Theory, AutoFakeData]
        public void Given_SelectedCountryCitySettingsNotAvailable__Then_CountryCityValuesNotSet_CityListNotPopulated(
            [Frozen] Mock<IUserSettings> userSettingsMock,
            [Frozen] Mock<IOptions<List<LocationSettings>>> locationSettingsOptionsMock,
            UserSettingsViewModel sut)
        {
            userSettingsMock.Setup(uset => uset.SelectedCountry).Returns((string)null);
            userSettingsMock.Setup(uset => uset.SelectedCity).Returns((string)null);

            locationSettingsOptionsMock.Setup(lso => lso.Value).Returns(GetDefaultLocationSettings());
            
            // --- Act
            sut.HandleAsync(new SettingsRequestedEventData(), CancellationToken.None);

            // --- Assert
            Assert.Null(sut.SelectedCountry);
            Assert.Null(sut.SelectedCity);
            Assert.Empty(sut.Cities);
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
}