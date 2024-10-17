using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace WeatherReport.UnitTests;

public class AutoFakeData : AutoDataAttribute
{
    public AutoFakeData()
        : base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true, 
            });
            fixture.OmitAutoProperties = true;
            return fixture;
        })
    {
    }
}
