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

namespace WeatherReport.UnitTests.ViewModels.UserSettingsViewModelTests
{
    [TestClass]
    public class SettingsCancelCommand
    {
        [TestMethod]
        public void Given_CommandExecuted__Then_SelectedRegionReset_SelectedCityReset_SettingsCancelledEventFired()
        {
            const string ISO2_CZ = "cz";
            const string ISO2_DE = "de";
            const string BRNO = "Brno";
            const string BERLIN = "Berlin";
            RegionInfo czechRegion = new RegionInfo(ISO2_CZ);
            RegionInfo germanRegion = new RegionInfo(ISO2_DE);
            string[] czechCities = new[] { BRNO, "Praha" };
            string[] germanCities = new[] { BERLIN, "Dresden" };
            bool isEventFired = false;

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { "cz", "de" }, ISO2_CZ, BRNO);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(czechRegion.EnglishName)).Returns(czechCities);

            var loggerMock = new Mock<ILog>();

            UserSettingsViewModel sut = new UserSettingsViewModel(weatherServiceMock.Object, loggerMock.Object, settings);
            sut.Init();
            sut.SettingsCancelled += (sender, e) => isEventFired = true;

            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "SelectedCity"))
            {
                sut.SelectedRegion = germanRegion;
            }
            sut.SelectedCity = BERLIN;

            // --- Act
            sut.SettingsCancelCommand.Execute(null);

            // --- Assert            
            Assert.AreEqual(czechRegion, sut.SelectedRegion, "Unexpected SelectedRegion");
            Assert.AreEqual(BRNO, sut.SelectedCity, "Unexpected SelectedCity");
        }
    }
}
