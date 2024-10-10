using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit;

namespace WeatherReport.UnitTests;

public class AutoFakeData : AutoDataAttribute
{
    public AutoFakeData()
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoFakeItEasyCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });
            return fixture;
        })
    {
    }
}
