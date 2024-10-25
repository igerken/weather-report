using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WeatherReport.WinApp.Converters;

public class IEnumerableOfPointsToPointCollectionConverter : IValueConverter
{
    private static readonly PointCollection EmptyPointCollection = new PointCollection();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(PointCollection))
            throw new InvalidOperationException("The target must be a PointCollection");

        IEnumerable<Point>? enumerableValue = value as IEnumerable<Point>;
        if(enumerableValue != null)
            return new PointCollection(enumerableValue);
        return EmptyPointCollection;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ConvertBack should never be called");
    }
}