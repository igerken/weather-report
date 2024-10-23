using System.Threading;
using System.Threading.Tasks;

using AutoFixture.Xunit2;
using Caliburn.Micro;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

using WeatherReport.Core;
using WeatherReport.Core.Events;
using WeatherReport.WinApp;
using WeatherReport.WinApp.Data;
using System;
using WeatherReport.WinApp.Events;

namespace WeatherReport.UnitTests.WeatherUpdateServiceTests;

public class HandleLocationChanged
{
    private const int PRECISION = 4;

    [Theory, AutoFakeData]
    public void Given_WeatherServiceResponseOk__Then_WeatherUpdatedMessagePublished([Frozen] Mock<IWeatherService> weatherServiceMock, 
        [Frozen] Mock<IEventAggregator> eventAggregatorMock, [Frozen] Mock<IOptions<AppSettings>> appSettingsMock, 
        Mock<ILocation> locationMock, Mock<IWeatherInfo> weatherInfoMock,
        WeatherUpdateService sut)
    {
        IWeatherInfo? weatherUpdatedInfo = null;

        appSettingsMock.Setup(op => op.Value).Returns(new AppSettings { RefreshIntervalSeconds = 60 });
        weatherServiceMock.Setup(ws => ws.GetWeather(locationMock.Object)).Returns(Task.FromResult(weatherInfoMock.Object));

        eventAggregatorMock.Setup(ea => ea.PublishAsync(It.IsAny<IWeatherUpdated>(), It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()))
            .Callback<object, Func<Func<Task>, Task>, CancellationToken>((wupd, _1, _2) => { weatherUpdatedInfo = (wupd as IWeatherUpdated)?.Weather;})
            .Returns(Task.CompletedTask);

        //--- Act
        sut.HandleAsync(new LocationChanged(locationMock.Object), CancellationToken.None);

        //--- Assert
        Assert.NotNull(weatherUpdatedInfo);
        Assert.Equal(weatherInfoMock.Object.Temperature.Value, weatherUpdatedInfo.Temperature.Value, PRECISION);
    }
}