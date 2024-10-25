using System;
using Xunit;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.ScrewViewModelTests;

public class RecalculateSlitCoordinates
{
    private const int PRECISION = 4;

    [Fact]
    public void Given_Direction0Degrees()
    {
        const double SIZE = 100.0;
        double dir = 0.0;

        ScrewViewModel sut = new ScrewViewModel(SIZE);

        //--- Act
        sut.RecalculateSlitCoordinates(dir);

        //--- Assert
        double halfSize = SIZE / 2.0;
        Assert.Equal(halfSize, sut.SlitX1, PRECISION);
        Assert.Equal(0.0, sut.SlitY1, PRECISION);
        Assert.Equal(halfSize, sut.SlitX2, PRECISION);
        Assert.Equal(SIZE, sut.SlitY2, PRECISION);
    }

    [Fact]
    public void Given_DirectionPlus45Degrees()
    {
        const double SIZE = 100;
        double dir = Math.PI / 4.0;

        ScrewViewModel sut = new ScrewViewModel(SIZE);

        //--- Act
        sut.RecalculateSlitCoordinates(dir);

        //--- Assert
        double halfSize = SIZE / 2.0;
        double cathetus = halfSize / Math.Sqrt(2.0);
        Assert.Equal(halfSize + cathetus, sut.SlitX1, PRECISION);
        Assert.Equal(halfSize - cathetus, sut.SlitY1, PRECISION);
        Assert.Equal(halfSize - cathetus, sut.SlitX2, PRECISION);
        Assert.Equal(halfSize + cathetus, sut.SlitY2, PRECISION);
    }

    [Fact]
    public void Given_DirectionPlus90Degrees()
    {
        const double SIZE = 100.0;
        double dir = Math.PI / 2.0;

        ScrewViewModel sut = new ScrewViewModel(SIZE);

        //--- Act
        sut.RecalculateSlitCoordinates(dir);

        //--- Assert
        double halfSize = SIZE / 2.0;
        Assert.Equal(SIZE, sut.SlitX1, PRECISION);
        Assert.Equal(halfSize, sut.SlitY1, PRECISION);
        Assert.Equal(0.0, sut.SlitX2, PRECISION);
        Assert.Equal(halfSize, sut.SlitY2, PRECISION);
    }

    [Fact]
    public void Given_DirectionMinus90Degrees()
    {
        const double SIZE = 100.0;
        double dir = -Math.PI / 2.0;

        ScrewViewModel sut = new ScrewViewModel(SIZE);

        //--- Act
        sut.RecalculateSlitCoordinates(dir);

        //--- Assert
        double halfSize = SIZE / 2.0;
        Assert.Equal(0.0, sut.SlitX1, PRECISION);
        Assert.Equal(halfSize, sut.SlitY1, PRECISION);
        Assert.Equal(SIZE, sut.SlitX2, PRECISION);
        Assert.Equal(halfSize, sut.SlitY2, PRECISION);
    }
}