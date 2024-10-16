using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Timers;
/*
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using WeatherReport.DataModel;
using WeatherReport.Interfaces;
using WeatherReport.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.WeatherViewModelTests
{
    [TestClass]
    public class SelectedRegion
    {
        [TestMethod]
        public void Given_SelectedRegionChanged__Then_CitiesUpdated()
        {
            const string ISO2_CZ = "cz";
            const string ISO2_DE = "de";
            const string SELECTED_CITY_INITIAL = "";
            RegionInfo czechRegion = new RegionInfo(ISO2_CZ);
            RegionInfo germanRegion = new RegionInfo(ISO2_DE);
            string[] czechCities = new[] { "Brno", "Ostrava", "Praha" };
            string[] germanCities = new[] { "Berlin", "Dresden" };

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { ISO2_CZ, ISO2_DE }, ISO2_CZ, SELECTED_CITY_INITIAL);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(czechRegion.EnglishName)).Returns(czechCities);
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(germanRegion.EnglishName)).Returns(germanCities);

            var loggerMock = new Mock<ILog>();

            UserSettingsViewModel sut = new UserSettingsViewModel(weatherServiceMock.Object, loggerMock.Object, settings);

            // --- Act
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "WeatherInfo"))
            {
                sut.SelectedRegion = germanRegion;
                monitor.WaitForPropertyChange();
            }

            // --- Assert            
            Assert.AreEqual(germanCities.Length, sut.Cities.Count, "Expected the count of Cities to correspond to the German region");
            Assert.IsTrue(!sut.Cities.Except(germanCities).Any(), "Expected Cities to correspond to the German region");
        }

        [TestMethod]
        public void Given_SelectedRegionChanged_CityListRetrievalFailed__Then_CityListAvailableIsFalse_ErrorMessageIsNotEmpty_ErrorIsLogged()
        {
            const string ISO2_CZ = "cz";
            const string ISO2_DE = "de";
            const string SELECTED_CITY_INITIAL = "";
            RegionInfo czechRegion = new RegionInfo(ISO2_CZ);
            RegionInfo germanRegion = new RegionInfo(ISO2_DE);
            Services.WeatherServiceException expectedException = new Services.WeatherServiceException(
                Services.WeatherServiceExceptionReason.CityListInvalidData, "Fake exception");

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { ISO2_CZ, ISO2_DE }, ISO2_CZ, SELECTED_CITY_INITIAL);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(It.IsAny<string>()))
                .Throws(expectedException);

            var loggerMock = new Mock<ILog>();
            loggerMock.Setup(log => log.Error(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<object, Exception>((m, e) => { Console.WriteLine(m.ToString()); Console.WriteLine(e.ToString()); });

            UserSettingsViewModel sut = new UserSettingsViewModel(weatherServiceMock.Object, loggerMock.Object, settings);

            // --- Act
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "CityListRetrievalErrorMessage"))
            {
                sut.SelectedRegion = germanRegion;
                monitor.WaitForPropertyChange();
            }

            // --- Assert            
            Assert.IsFalse(sut.IsCityListAvailable, "Unexpected IsCityListAvailable");
            Assert.IsTrue(sut.CityListRetrievalErrorMessage.Length > 0, "Unexpected CityListRetrievalErrorMessage");
            loggerMock.Verify(log => log.Error(It.IsAny<object>(), expectedException), "The expected exception was not logged");
        }
    }
}
*/