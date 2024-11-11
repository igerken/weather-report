using System.Windows.Media;
using WeatherReport.WinApp.Converters;
using WeatherReport.WinApp.ViewModels;
using Xunit;

namespace WeatherReport.UnitTests.Converters.HsvColorToColorConverterTests;

public class Convert
{
    [Fact]
    public void WhenCalledWithHsvColorRed_ReturnsColorRed()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvRed = new HsvColor(0, 1, 1);

        // Act
        var result = converter.Convert(hsvRed, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(255, 0, 0), result);
    }   

    // Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the blue color
    [Fact]
    public void WhenCalledWithHsvColorBlue_ReturnsColorBlue()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvBlue = new HsvColor(240, 1, 1);

        // Act
        var result = converter.Convert(hsvBlue, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(0, 0, 255), result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for HsvColor which represents the black color (no saturation)
    [Fact]
    public void WhenCalledWithHsvColorBlack_ReturnsColorBlack()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvBlack = new HsvColor(0, 0, 0);
        var rgbBlack = Color.FromRgb(0, 0, 0);

        // Act
        var result = converter.Convert(hsvBlack, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(rgbBlack, result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the white color (full saturation and brightness)
    [Fact]
    public void WhenCalledWithHsvColorWhite_ReturnsColorWhite()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvWhite = new HsvColor(0, 0, 1);
        var rgbWhite = Color.FromRgb(255, 255, 255);

        // Act
        var result = converter.Convert(hsvWhite, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(rgbWhite, result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for  HsvColor which represents the half saturated red color
    [Fact]
    public void WhenCalledWithHsvColorRedHalfSaturatedFullBright_ReturnsColorRed()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(0, 0.5f, 1);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(255, 127, 127), result);
    }
    
    // Unit test for Convert method of HsvColorToColorConverter class for HsvColor which represents the half saturated green color, half bright
    [Fact]
    public void WhenCalledWithHsvColorGreenHalfSaturatedHalfBright_ReturnsColorGreen()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvColor = new HsvColor(120, 0.5f, 0.5f);

        // Act
        var result = converter.Convert(hsvColor, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(63, 127, 63), result);
    }

    // Unit test for Convert method of HsvColorToColorConverter class for HsvColor which represents neutral grey color, half bright
    [Fact]
    public void WhenCalledWithHsvColorGreyHalfBright_ReturnsColorGrey()
    {
        // Arrange
        var converter = new HsvColorToColorConverter();
        var hsvGrey = new HsvColor(0, 0, 0.5f);

        // Act
        var result = converter.Convert(hsvGrey, typeof(Color), null, null);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Color.FromRgb(127, 127, 127), result);
    }
}