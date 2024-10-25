using System;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture.Xunit2;
using Caliburn.Micro;
using Moq;
using Xunit;

using WeatherReport.Core;
using WeatherReport.WinApp;
using WeatherReport.WinApp.Events;

namespace WeatherReport.UnitTests.WeatherUpdateServiceTests;

public class HandleLocationChanged
{
    private const int Precision = 4;
    private const int SemaphoreTimeoutMs = 2000;    

    [Theory, AutoFakeData]
    public async Task Given_WeatherServiceResponseOk__Then_WeatherUpdatedMessagePublished(
        [Frozen] Mock<IWeatherService> weatherServiceMock, 
        [Frozen] Mock<IEventAggregator> eventAggregatorMock,
        Mock<ILocation> locationMock, Mock<IWeatherInfo> weatherInfoMock,
        WeatherUpdateService sut)
    {
        IWeatherInfo? weatherUpdatedInfo = null;
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        TimeSpan serviceResponseTime = TimeSpan.FromMilliseconds(50);

        // Simulate delayed service response
        weatherServiceMock.Setup(ws => ws.GetWeather(locationMock.Object)).ReturnsAsync(weatherInfoMock.Object, serviceResponseTime);

        // Setup wait for the call of event aggregator
        eventAggregatorMock.Setup(ea => ea.PublishAsync(It.IsAny<WeatherUpdated>(), It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()))
            .Callback<object, Func<Func<Task>, Task>, CancellationToken>((wupd, _1, _2) => 
                {
                    weatherUpdatedInfo = (wupd as WeatherUpdated)?.Weather;
                    semaphoreSlim.Release();
                })
            .Returns(Task.CompletedTask);

        //--- Act
        await sut.HandleAsync(new LocationChanged(locationMock.Object), CancellationToken.None);
        await semaphoreSlim.WaitAsync(SemaphoreTimeoutMs);

        //--- Assert
        Assert.NotNull(weatherUpdatedInfo);
        Assert.True(weatherInfoMock.Object.Temperature.HasValue);
        Assert.True(weatherUpdatedInfo.Temperature.HasValue);
        Assert.Equal(weatherInfoMock.Object.Temperature.Value, weatherUpdatedInfo.Temperature.Value, Precision);
    }

    [Theory, AutoFakeData]
    public async Task Given_WeatherServiceFails__Then_WeatherUpdateFaileddMessagePublished(
        [Frozen] Mock<IWeatherService> weatherServiceMock, 
        [Frozen] Mock<IEventAggregator> eventAggregatorMock,
        Mock<ILocation> locationMock,
        WeatherUpdateService sut)
    {
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        TimeSpan serviceResponseTime = TimeSpan.FromMilliseconds(50);
        WeatherServiceFailureReason failureReason = WeatherServiceFailureReason.WeatherInfoUnavailable;
        WeatherServiceFailureReason? publishedFailureReason = null;

        // Simulate delayed service response
        weatherServiceMock
            .Setup(ws => ws.GetWeather(It.IsAny<ILocation>()))
            .ThrowsAsync(new WeatherServiceException(failureReason, string.Empty), serviceResponseTime);

        // Setup wait for the call of event aggregator
        eventAggregatorMock.Setup(ea => ea.PublishAsync(It.IsAny<WeatherUpdateFailed>(), It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()))
            .Callback<object, Func<Func<Task>, Task>, CancellationToken>((wupd, _1, _2) => 
                {
                    publishedFailureReason = (wupd as WeatherUpdateFailed)?.Reason;
                    semaphoreSlim.Release();
                })
            .Returns(Task.CompletedTask);

        //--- Act
        await sut.HandleAsync(new LocationChanged(locationMock.Object), CancellationToken.None);
        await semaphoreSlim.WaitAsync(SemaphoreTimeoutMs);

        //--- Assert
        Assert.True(publishedFailureReason.HasValue);
        Assert.Equal(publishedFailureReason.Value, failureReason);
    }
}