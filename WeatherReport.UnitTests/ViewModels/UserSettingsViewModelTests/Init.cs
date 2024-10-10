using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Caliburn.Micro;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using WeatherReport.DataModel;
using WeatherReport.Interfaces;
using WeatherReport.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.UserSettingsViewModelTests
{
    [TestClass]
    public class Init
    {
        [TestMethod]
        public void Given_SelectedCountryCitySettingsAvailable__Then_CountryCityValuesSet_CityListPopulated()
        {
            const string CZECH_REP_ISO2 = "cz";
            const string BRNO = "Brno";
            RegionInfo czechRegion = new RegionInfo(CZECH_REP_ISO2);
            string[] czechCities = new[] { BRNO, "Praha" };

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { CZECH_REP_ISO2, "de" }, CZECH_REP_ISO2, BRNO);

            var globalDataContainerMock = new Mock<IGlobalDataContainer>();
            globalDataContainerMock.Setup(dc => dc.YrLocationData).Returns(czechCities);

            var eventAggregatorMock = new Mock<IEventAggregator>();
            var loggerMock = new Mock<log4net.ILog>();

            UserSettingsViewModel sut = new UserSettingsViewModel(globalDataContainerMock.Object, eventAggregatorMock.Object, loggerMock.Object, settings);

            // --- Act
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "WeatherInfo"))
            {
                sut.Init();
                monitor.WaitForPropertyChange();
            }

            // --- Assert
            Assert.AreEqual(czechRegion, sut.SelectedRegion, "Unexpected SelectedRegion");
            Assert.AreEqual(BRNO, sut.SelectedCity, "Unexpected SelectedCity");
            Assert.IsTrue(sut.Cities.SequenceEqual(czechCities), "Unexpected Cities");
        }

        [TestMethod]
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

    }
}
