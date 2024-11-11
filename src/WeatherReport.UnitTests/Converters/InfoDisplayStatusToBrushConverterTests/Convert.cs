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
        Color errorColor = Color.FromRgb(255, 0, 0);
        var converter = new InfoDisplayStatusToBrushConverter();
        converter.ErrorColor = errorColor;
        converter.NormalColor = Color.FromRgb(0, 255, 0);

        // Act
        var result = converter.Convert(InfoDisplayStatus.Error, typeof(Brush), null, null);

        // Assert
        Assert.IsAssignableFrom<Brush>(result);
        Assert.Equal(errorColor, ((SolidColorBrush)result).Color);
    }

    // Unit test for Convert method of InfoDisplayStatusToBrushConverter class for InfoDisplayStatus which represents the normal status
    [Fact]
    public void Convert_WhenCalledWithInfoDisplayStatusNormal_ReturnsNormalBrush()
    {
        // Arrange
        Color normalColor = Color.FromRgb(0, 255, 0);
        var converter = new InfoDisplayStatusToBrushConverter();
        converter.ErrorColor = Color.FromRgb(255, 0, 0);
        converter.NormalColor = normalColor;

        // Act
        var result = converter.Convert(InfoDisplayStatus.Normal, typeof(Brush), null, null);

        // Assert
        Assert.IsAssignableFrom<Brush>(result);
        Assert.Equal(normalColor, ((SolidColorBrush)result).Color);
    }
}