using System.Windows.Media;
using WeatherReport.WinApp.Converters;
using WeatherReport.WinApp.ViewModels;
using Xunit;

namespace WeatherReport.UnitTests.Converters.HsvColorToColorConverterTests;

public class Convert
{
    [Fact]
    public void Convert_WhenCalledWithHsvColorRed_ReturnsColorRed()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(0, 1, 1);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(255, 0, 0), result);
    }   

    // Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the blue color
    [Fact]
    public void Convert_WhenCalledWithHsvColorBlue_ReturnsColorBlue()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(240, 1, 1);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(0, 0, 255), result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for HsvColor which represents the black color (no saturation)
    [Fact]
    public void Convert_WhenCalledWithHsvColorBlack_ReturnsColorBlack()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(0, 0, 0);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(0, 0, 0), result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the white color (full saturation and brightness)
    [Fact]
    public void Convert_WhenCalledWithHsvColorWhite_ReturnsColorWhite()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(0, 1, 1);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(255, 255, 255), result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the half saturated red color, half bright
    [Fact]
    public void Convert_WhenCalledWithHsvColorRedHalfSaturatedHalfBright_ReturnsColorRed()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(0, 0.5f, 0.5f);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(128, 0, 0), result);
    }
    
// Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the half saturated green color, half bright
    [Fact]
    public void Convert_WhenCalledWithHsvColorGreenHalfSaturatedHalfBright_ReturnsColorGreen()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(120, 0.5f, 0.5f);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(64, 128, 64), result);
    }

    
}