using System;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture.Xunit2;
using Caliburn.Micro;
using Moq;
using WeatherReport.WinApp.Events;
using WeatherReport.WinApp.Interfaces;
using WeatherReport.WinApp.ViewModels;
using Xunit;

namespace WeatherReport.UnitTests.ViewModels.MainViewModelTests;

public class HandleSettingsOkayed
{
    // Unit test for HandleAsync method of MainViewModel class for SettingsOkayed message, verify that DisplayedCity is set correctly and user settings are saved, inject SUT and its dependencies into the test method using AutoFakeData attribute
    [Theory, AutoFakeData]
    public async Task Given_SettingsOkayedMessage__Then_DisplayedCityIsSetCorrectlyAndUserSettingsAreSaved(
        [Frozen] Mock<IUserSettings> userSettingsMock,
        [Frozen] Mock<IEventAggregator> eventAggregatorMock,
        MainViewModel sut)
    {
        const string city = "New York";
        const string country = "US";

        // Setup user settings
        userSettingsMock.SetupProperty(us => us.SelectedCity, city);
        userSettingsMock.SetupProperty(us => us.SelectedCountry, country);

        //--- Act
        await sut.HandleAsync(new SettingsOkayed(country, city), CancellationToken.None);

        //--- Assert
        Assert.Equal(city, sut.DisplayedCity);
        userSettingsMock.Verify(us => us.Save(), Times.Once);
        eventAggregatorMock.Verify(ea => ea.PublishAsync(
            It.Is<LocationChanged>(lc => lc.Location.City == city && lc.Location.Country == country), 
            It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
