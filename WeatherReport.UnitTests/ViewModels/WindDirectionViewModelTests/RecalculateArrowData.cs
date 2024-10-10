using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WeatherReport.ViewModels;

namespace WeatherReport.UnitTests.ViewModels.WindDirectionViewModelTests
{
    [TestClass]
    public class RecalculateArrowData
    {
        [TestMethod]
        public void Given_WindFromSouthWest__Then_ArrowLeftWingIsBighterThenRightWing()
        {
            const double WIND_DIRECTION_SOUTH_WEST = 1.25 * Math.PI;
            const double WIND_SPEED_MODERATE = 5.0;

            WindDirectionViewModel sut = new WindDirectionViewModel(50.0, 50.0, 40.0);

            // --- Act
            sut.RecalculateArrowData(WIND_SPEED_MODERATE, WIND_DIRECTION_SOUTH_WEST);

            Assert.IsTrue(sut.ArrowLeftWingColor.Value > sut.ArrowRightWingColor.Value,
                "Expected the left wing to be brighter than the right wing");
        }

        [TestMethod]
        public void Given_WindFromNorthEast__Then_ArrowRightWingIsBighterThenLeftWing()
        {
            const double WIND_DIRECTION_NORTH_EAST = 0.25 * Math.PI;
            const double WIND_SPEED_MODERATE = 5.0;

            WindDirectionViewModel sut = new WindDirectionViewModel(50.0, 50.0, 40.0);

            // --- Act
            sut.RecalculateArrowData(WIND_SPEED_MODERATE, WIND_DIRECTION_NORTH_EAST);

            Assert.IsTrue(sut.ArrowLeftWingColor.Value < sut.ArrowRightWingColor.Value,
                "Expected the right wing to be brighter than the left wing");
        }
    }
}
