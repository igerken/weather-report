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
    public class SettingsOkCommand
    {
        private const bool ASSERT_IGNORES_CASE = true;

        [TestMethod]
        public void Given_CommandExecuted__Then_SettingsSaved_SettingsOkayedEventFired()
        {
            const string ISO2_CZ = "cz";
            const string ISO2_DE = "de";
            const string BRNO = "Brno";
            const string BERLIN = "Berlin";
            RegionInfo czechRegion = new RegionInfo(ISO2_CZ);
            RegionInfo germanRegion = new RegionInfo(ISO2_DE);
            string[] czechCities = new[] { BRNO, "Praha" };
            string[] germanCities = new[] { BERLIN, "Dresden" };

            RegionInfo eventArgsRegion = null;
            string eventArgsCity = null;

            DummyWeatherSettings settings = new DummyWeatherSettings(
                new StringCollection { "cz", "de" }, ISO2_CZ, BRNO);

            var weatherServiceMock = new Mock<IWeatherService>();
            weatherServiceMock.Setup(ws => ws.GetCitiesByCountry(czechRegion.EnglishName)).Returns(czechCities);

            var loggerMock = new Mock<ILog>();

            UserSettingsViewModel sut = new UserSettingsViewModel(weatherServiceMock.Object, loggerMock.Object, settings);
            sut.Init();
            sut.SettingsOkayed += (sender, e) =>
                {
                    eventArgsRegion = e.Data1;
                    eventArgsCity = e.Data2;
                };

            using (PropertyChangeMonitor monitor = new PropertyChangeMonitor(sut, "SelectedCity"))
            {
                sut.SelectedRegion = germanRegion;
            }
            sut.SelectedCity = BERLIN;

            // --- Act
            sut.SettingsOkCommand.Execute(null);

            // --- Assert
            Assert.AreEqual(ISO2_DE, settings.SelectedCountry, ASSERT_IGNORES_CASE, "Unexpected SelectedCountry in settings");
            Assert.AreEqual(BERLIN, settings.SelectedCity, ASSERT_IGNORES_CASE, "Unexpected SelectedCity in settings");
            Assert.AreEqual(germanRegion, eventArgsRegion, "Unexpected eventArgsRegion");
            Assert.AreEqual(BERLIN, eventArgsCity, ASSERT_IGNORES_CASE, "Unexpected eventArgsCity");
        }
    }
}
