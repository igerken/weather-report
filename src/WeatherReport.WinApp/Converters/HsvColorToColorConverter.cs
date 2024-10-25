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
            return GetColorFromHSV(hsvValue.Hue, hsvValue.Saturation, hsvValue.Value);
        return DefaultColor;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ConvertBack should never be called");
    }

    private static Color GetColorFromHSV(float h, float s, float v)
    {
        double r = 0.0, g = 0.0, b = 0.0;
        double p, q, f, t;
        int i;

        if (s == 0.0F)
        {
            r = v;
            g = r;
            b = r;
        }
        else
        {
            h = h / 60;
            i = (int)h;
            f = h - i;
            p = v * (1 - s);
            q = v * (1 - (s * f));
            t = v * (1 - (s * (1 - f)));

            switch (i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:		// case 5:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }
        }

        return Color.FromScRgb(1, (float)r, (float)g, (float)b);
    }
}