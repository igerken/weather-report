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

    [Theory, AutoFakeData]
    public async Task Given_WeatherServiceResponseOk__Then_WeatherUpdatedMessagePublished(
        [Frozen] Mock<IWeatherService> weatherServiceMock, 
        [Frozen] Mock<IEventAggregator> eventAggregatorMock,
        Mock<ILocation> locationMock, Mock<IWeatherInfo> weatherInfoMock,
        WeatherUpdateService sut)
    {
        TimeSpan serviceResponseTime = TimeSpan.FromMilliseconds(50);

        // Simulate delayed service response
        weatherServiceMock.Setup(ws => ws.GetWeather(locationMock.Object)).ReturnsAsync(weatherInfoMock.Object, serviceResponseTime);

        //--- Act
        await sut.HandleAsync(new LocationChanged(locationMock.Object), CancellationToken.None);

        //--- Assert
        eventAggregatorMock.Verify(ea => ea.PublishAsync(
            It.Is<WeatherUpdated>(wu => IsValid(weatherInfoMock.Object.Temperature.Value, wu.Weather.Temperature)), 
            It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private bool IsValid(double expected, double? actual)
    {
        return actual.HasValue && Math.Abs(expected - actual.Value) < Math.Pow(10, -Precision);
    }

    [Theory, AutoFakeData]
    public async Task Given_WeatherServiceFails__Then_WeatherUpdateFailedMessagePublished(
        [Frozen] Mock<IWeatherService> weatherServiceMock, 
        [Frozen] Mock<IEventAggregator> eventAggregatorMock,
        Mock<ILocation> locationMock,
        WeatherUpdateService sut)
    {
        TimeSpan serviceResponseTime = TimeSpan.FromMilliseconds(50);
        WeatherServiceFailureReason failureReason = WeatherServiceFailureReason.AccessDenied;

        // Simulate delayed service response
        weatherServiceMock
            .Setup(ws => ws.GetWeather(It.IsAny<ILocation>()))
            .ThrowsAsync(new WeatherServiceException(failureReason, string.Empty), serviceResponseTime);

        //--- Act
        await sut.HandleAsync(new LocationChanged(locationMock.Object), CancellationToken.None);

        //--- Assert
        eventAggregatorMock.Verify(ea => ea.PublishAsync(
            It.Is<WeatherUpdateFailed>(waf => waf.Reason == failureReason), 
            It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // Unit test for HandleAsync method of WeatherUpdateService class for Exception thrown when calling weather service, inject SUT into the test method using AutoFakeData attribute
    [Theory, AutoFakeData]
    public async Task Given_WeatherServiceThrowsException__Then_WeatherUpdateFailedMessagePublishedWithReasonWeatherInfoUnavailable(
        [Frozen] Mock<IWeatherService> weatherServiceMock, 
        [Frozen] Mock<IEventAggregator> eventAggregatorMock,
        Mock<ILocation> locationMock,
        WeatherUpdateService sut)
    {
        TimeSpan serviceResponseTime = TimeSpan.FromMilliseconds(50);

        // Simulate delayed service response
        weatherServiceMock
            .Setup(ws => ws.GetWeather(It.IsAny<ILocation>()))
            .ThrowsAsync(new Exception(), serviceResponseTime);

        //--- Act
        await sut.HandleAsync(new LocationChanged(locationMock.Object), CancellationToken.None);

        //--- Assert
        eventAggregatorMock.Verify(ea => ea.PublishAsync(
            It.Is<WeatherUpdateFailed>(waf => waf.Reason == WeatherServiceFailureReason.WeatherInfoUnavailable), 
            It.IsAny<Func<Func<Task>, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}