using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using WeatherReport.DataModel;
using WeatherReport.Interfaces;
using WeatherReport.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.WeatherViewModelTests
{
    [TestClass]
    public class Init
    {
        [TestMethod]
        public void Given_SelectedCountryCitySettingsAvailable__Then_CountryCityValuesSet_CityListPopulated_WeatherRetrieved()
        {
            const string CZECH_REP_ISO2 = "cz";
            const string BRNO = "Brno";
            RegionInfo czechRegion = new RegionInfo(CZECH_REP_ISO2);
            string[] czechCities = new[] { BRNO, "Praha" };
            WeatherInfo brnoWeather = new WeatherInfo(15.0, 3.1, 5.0);

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { CZECH_REP_ISO2, "de" }, CZECH_REP_ISO2, BRNO);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(czechRegion.EnglishName)).Returns(czechCities);
            weatherServiceMock.Setup(ws => ws.GetWeather(BRNO, czechRegion.EnglishName))
                .Returns(brnoWeather);

            var loggerMock = new Mock<ILog>();

            WeatherViewModel sut = new WeatherViewModel(weatherServiceMock.Object, loggerMock.Object, settings);         

            // --- Act
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "WeatherInfo"))
            {
                sut.Init();
                monitor.WaitForPropertyChange();
            }            
                        
            // --- Assert
            Assert.AreEqual(czechRegion, sut.DisplayedRegion, "Unexpected DisplayedRegion");
            Assert.AreEqual(BRNO, sut.DisplayedCity, "Unexpected DisplayedCity");
            Assert.AreEqual(brnoWeather, sut.WeatherInfo, "Unexpected WeatherInfo");
        }

        [TestMethod]
        public void Given_SelectedCountryCitySettingsNotAvailable__Then_CountryCityValuesNotSet_CityListNotPopulated_SettingsWidgetVisible()
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

            WeatherViewModel sut = new WeatherViewModel(weatherServiceMock.Object, loggerMock.Object, settings);

            // --- Act
            sut.Init();

            // --- Assert
            Assert.IsNull(sut.DisplayedRegion, "Unexpected DisplayedRegion");
            Assert.IsNull(sut.DisplayedCity, "Unexpected DisplayedCity");
        }

        [TestMethod]
        public void Given_WeatherInfoRetrievalFailed__Then_ErrorMessageDisplayed_ErrorLogged()
        {
            const string ISO2_CZ = "cz";
            const string BRNO = "Brno";
            const int SECONDS_NUMEROUS = 1000;
            RegionInfo czechRegion = new RegionInfo(ISO2_CZ);
            string[] czechCities = new[] { BRNO, "Praha" };
            Services.WeatherServiceException expectedException = new Services.WeatherServiceException(
                Services.WeatherServiceExceptionReason.WeatherInfoInvalidData, "Fake exception");

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { "cz", "de" }, ISO2_CZ, BRNO, SECONDS_NUMEROUS);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(czechRegion.EnglishName)).Returns(czechCities);
            weatherServiceMock.Setup(ws => ws.GetWeather(BRNO, czechRegion.EnglishName))
                .Throws(expectedException);

            var loggerMock = new Mock<ILog>();
            loggerMock.Setup(log => log.Error(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<object, Exception>((m, e) => { Console.WriteLine(m.ToString()); Console.WriteLine(e.ToString()); });

            WeatherViewModel sut = new WeatherViewModel(weatherServiceMock.Object, loggerMock.Object, settings);            

            // --- Act         
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "InfoDisplayStatus"))
            {
                sut.Init();
                monitor.WaitForPropertyChange();
            }

            // --- Assert            
            Assert.AreEqual(InfoDisplayStatus.Error, sut.InfoDisplayStatus, "Unexpected InfoDisplayStatus");
            loggerMock.Verify(log => log.Error(It.IsAny<object>(), expectedException));
        }
    }
}
