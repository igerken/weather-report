using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp.Converters;

public class HsvColorToColorConverter : IValueConverter
{
    private static readonly Color DefaultColor = Color.FromArgb(0, 0, 0, 0);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Color))
            throw new InvalidOperationException("The target must be a Color");

        HsvColor? hsvValue = value as HsvColor;
        if (hsvValue != null)
            return GetColorFromHsv(hsvValue.Hue, hsvValue.Saturation, hsvValue.Value);
        return DefaultColor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ConvertBack should never be called");
    }

    private static Color GetColorFromHsv(float hue, float sat, float val)
    {
        if (sat == 0)
        {
            byte shade = (byte)(val * 255);
            return Color.FromRgb(shade, shade, shade);
        }

        float chroma = val * sat;
        float x = chroma * (1 - Math.Abs(hue / 60 % 2 - 1));
        float m = val - chroma;

        float r, g, b;
        if (hue < 60)
        {
            r = chroma;
            g = x;
            b = 0;
        }
        else if (hue < 120)
        {
            r = x;
            g = chroma;
            b = 0;
        }
        else if (hue < 180)
        {
            r = 0;
            g = chroma;
            b = x;
        }
        else if (hue < 240)
        {
            r = 0;
            g = x;
            b = chroma;
        }
        else if (hue < 300)
        {
            r = x;
            g = 0;
            b = chroma;
        }
        else
        {
            r = chroma;
            g = 0;
            b = x;
        }

        return Color.FromRgb((byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
    }
}