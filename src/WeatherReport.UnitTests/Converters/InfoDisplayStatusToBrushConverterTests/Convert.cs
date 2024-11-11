using System.Windows.Media;
using WeatherReport.WinApp.Converters;
using WeatherReport.WinApp.ViewModels;
using Xunit;

namespace WeatherReport.UnitTests.Converters.InfoDisplayStatusToBrushConverterTests;

public class Convert
{
    // Unit test for Convert method of InfoDisplayStatusToBrushConverter class for InfoDisplayStatus which represents the error status
    [Fact]
    public void Convert_WhenCalledWithInfoDisplayStatusError_ReturnsErrorBrush()
    {
        // Arrange
        var converter = new InfoDisplayStatusToBrushConverter();
        converter.ErrorColor = Color.FromRgb(255, 0, 0);
        converter.NormalColor = Color.FromRgb(0, 255, 0);

        // Act
        var result = converter.Convert(InfoDisplayStatus.Error, typeof(Brush), null, null);

        // Assert
        Assert.IsAssignableFrom<System.Windows.Media.Brush>(result);
        Assert.Equal(System.Windows.Media.Color.FromRgb(255, 0, 0), ((System.Windows.Media.SolidColorBrush)result).Color);
    }
}