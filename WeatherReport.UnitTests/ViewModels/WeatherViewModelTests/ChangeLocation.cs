using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using AutoFixture.Xunit;
using Moq;
using WeatherReport.Core;
using WeatherReport.DataModel;
using WeatherReport.Interfaces;
using WeatherReport.WinApp.ViewModels;
using Xunit;

namespace WeatherReport.UnitTests.ViewModels.WeatherViewModelTests
{
    public class ChangeLocation
    {
        [Theory, AutoFakeData]
        public void Given_CommandExecuted__Then_WeatherInfoRetrievedForSelectedCountryCity([Frozen] IWeatherService weatherServiceMock,
            MainViewModel sut)
        {            
            const string COUNTRY_CZ = "CZ";
            const string BRNO = "Brno";
            WeatherInfo brnoWeather = new WeatherInfo(15.0, 3.1, 5.0);

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { "cz", "de" });

            weatherServiceMock.Setup(ws => ws.GetWeather(It.Is<ILocation>(loc => loc.Country == COUNTRY_CZ && loc.City == BRNO)))
                .Returns(brnoWeather);

            var loggerMock = new Mock<ILog>();


            // --- Act
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "WeatherInfo"))
            {
                sut.ChangeLocation(czechRegion, BRNO);
                monitor.WaitForPropertyChange();
            }

            // --- Assert            
            Assert.AreEqual(brnoWeather, sut.WeatherInfo, "Unexpected WeatherInfo");
        }

        [Theory, AutoFakeData]
        public void Given_CommandExecuted__Then_DisplayedRegionSetToSelected_DisplayedCitySetToSelected()
        {
            const string BRNO = "Brno";
            RegionInfo czechRegion = new RegionInfo("cz");
            string[] czechCities = new[] { BRNO, "Praha" };

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { "cz", "de" });

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetWeather( )))
                .Returns(new WeatherInfo());

            var loggerMock = new Mock<ILog>();

            WeatherViewModel sut = new WeatherViewModel(weatherServiceMock.Object, loggerMock.Object, settings);
            sut.Init();

            // --- Act
            sut.ChangeLocation(czechRegion, BRNO);

            // --- Assert            
            Assert.AreEqual(czechRegion, sut.DisplayedRegion, "Unexpected DisplayedRegion");
            Assert.AreEqual(BRNO, sut.DisplayedCity, "Unexpected DisplayedCity");
        }

        [TestMethod]
        public void Given_WeatherInfoRetrievalFailed__Then_ErrorMessageDisplayed_ErrorLogged()
        {
            const string ISO2_CZ = "cz";
            const string BRNO = "Brno";
            const string PRAHA = "Praha";
            const int SECONDS_NUMEROUS = 1000;
            RegionInfo czechRegion = new RegionInfo(ISO2_CZ);
            string[] czechCities = new[] { BRNO, PRAHA };
            Services.WeatherServiceException expectedException = new Services.WeatherServiceException(
                Services.WeatherServiceExceptionReason.WeatherInfoInvalidData, "Fake exception");

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { ISO2_CZ, "de" }, ISO2_CZ, BRNO, SECONDS_NUMEROUS);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetWeather(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(expectedException);

            var loggerMock = new Mock<ILog>();
            loggerMock.Setup(log => log.Error(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<object, Exception>((m, e) => { Console.WriteLine(m.ToString()); Console.WriteLine(e.ToString()); });

            WeatherViewModel sut = new WeatherViewModel(weatherServiceMock.Object, loggerMock.Object, settings);

            // --- Act         
            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "InfoDisplayStatus"))
            {
                sut.ChangeLocation(czechRegion, PRAHA);
                monitor.WaitForPropertyChange();
            }

            // --- Assert            
            Assert.AreEqual(InfoDisplayStatus.Error, sut.InfoDisplayStatus, "Unexpected InfoDisplayStatus");
            loggerMock.Verify(log => log.Error(It.IsAny<object>(), expectedException));
        }
    }
}
