using AutoFixture.Xunit2;
using Moq;
using Xunit;

using WeatherReport.WinApp.ViewModels;
using WeatherReport.WinApp.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using WeatherReport.Core.Settings;
using WeatherReport.WinApp.Events;
using System.Threading;

namespace WeatherReport.UnitTests.ViewModels.UserSettingsViewModelTests
{
    public class HandleSettingsRequested
    {
        [Theory, AutoFakeData]
        public void Given_SelectedCountryCityUserSettingsAvailable__Then_CountryCityValuesSet_CityListPopulated(
            [Frozen] Mock<IUserSettings> userSettingsMock,
            [Frozen] Mock<IOptions<List<LocationSettings>>> locationSettingsOptionsMock,
            UserSettingsViewModel sut
        )
        {
            const string COUNTRY_CZECHIA = "CZ";
            const string CITY_BRNO = "Brno";

            List<LocationSettings> locationSettings = new List<LocationSettings>();

            userSettingsMock.Setup(uset => uset.SelectedCountry).Returns(COUNTRY_CZECHIA);
            userSettingsMock.Setup(uset => uset.SelectedCity).Returns(CITY_BRNO);

            locationSettingsOptionsMock.Setup(lso => lso.Value).Returns(locationSettings);

            
            // --- Act
            sut.HandleAsync(new SettingsRequestedEventData(), CancellationToken.None);

            // --- Assert
            Assert.Equal(COUNTRY_CZECHIA, sut.SelectedCountry);
            Assert.Equal(CITY_BRNO, sut.SelectedCity);
        }

        /*
        public void Given_SelectedCountryCitySettingsNotAvailable__Then_CountryCityValuesNotSet_CityListNotPopulated()
        {
            const string CZECH_REP_ISO2 = "cz";
            const string BRNO = "Brno";
            RegionInfo czechRegion = new RegionInfo(CZECH_REP_ISO2);
            string[] czechCities = new[] { BRNO, "Praha" };
            WeatherInfo brnoWeather = new WeatherInfo(15.0, 3.1, 5.0);

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { CZECH_REP_ISO2, "de" });

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(czechRegion.EnglishName)).Returns(czechCities);
            weatherServiceMock.Setup(ws => ws.GetWeather(BRNO, czechRegion.EnglishName))
                .Returns(brnoWeather);

            var loggerMock = new Mock<ILog>();

            UserSettingsViewModel sut = new UserSettingsViewModel(weatherServiceMock.Object, loggerMock.Object, settings);

            // --- Act
            sut.Init();

            // --- Assert
            Assert.IsNull(sut.SelectedRegion, "Unexpected SelectedRegion");
            Assert.IsNull(sut.SelectedCity, "Unexpected SelectedCity");
            Assert.AreEqual(0, sut.Cities.Count, "Unexpected Cities count");
        }
*/
    }
}