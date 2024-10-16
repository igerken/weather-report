using System;
using Xunit;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.WindDirectionViewModelTests
{
    public class RecalculateArrowData
    {
        [Theory, AutoFakeData]
        public void Given_WindFromSouthWest__Then_ArrowLeftWingIsBighterThenRightWing(WindDirectionViewModel sut)
        {
            const double WIND_DIRECTION_SOUTH_WEST = 1.25 * Math.PI;
            const double WIND_SPEED_MODERATE = 5.0;

            // --- Act
            sut.RecalculateArrowData(WIND_SPEED_MODERATE, WIND_DIRECTION_SOUTH_WEST);

            Assert.True(sut.ArrowLeftWingColor.Value > sut.ArrowRightWingColor.Value,
                "Expected the left wing to be brighter than the right wing");
        }

        [Theory, AutoFakeData]
        public void Given_WindFromNorthEast__Then_ArrowRightWingIsBighterThenLeftWing(WindDirectionViewModel sut)
        {
            const double WIND_DIRECTION_NORTH_EAST = 0.25 * Math.PI;
            const double WIND_SPEED_MODERATE = 5.0;

            // --- Act
            sut.RecalculateArrowData(WIND_SPEED_MODERATE, WIND_DIRECTION_NORTH_EAST);

            Assert.True(sut.ArrowLeftWingColor.Value < sut.ArrowRightWingColor.Value,
                "Expected the right wing to be brighter than the left wing");
        }
    }
}