using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

using WeatherReport.WinApp.ViewModels;

namespace WeatherReport.WinApp.Converters
{
    public class InfoDisplayStatusToBrushConverter : IValueConverter
    {
        private Color _normalColor;
        private Color _errorColor;

        private Brush _normalBrush;
        private Brush _errorBrush;

        public Color NormalColor
        {
            get { return _normalColor; }
            set
            {
                if (_normalColor != value)
                {
                    _normalColor = value;
                    _normalBrush = new SolidColorBrush(_normalColor);
                }
            }
        }
        public Color ErrorColor
        {
            get { return _errorColor; }
            set
            {
                if (_errorColor != value)
                {
                    _errorColor = value;
                    _errorBrush = new SolidColorBrush(_errorColor);
                }
            }
        }

        public InfoDisplayStatusToBrushConverter()
        {
            Color white = Color.FromScRgb(1.0f, 1.0f, 1.0f, 1.0f);
            Brush whiteBrush = new SolidColorBrush(white);
            _normalColor = white;
            _errorColor = white;
            _normalBrush = whiteBrush;
            _errorBrush = whiteBrush;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("The target must be a Brush");

            if (!(value is InfoDisplayStatus))
                throw new InvalidOperationException("The value must be a InfoDisplayStatus");

            InfoDisplayStatus status = (InfoDisplayStatus)value;
            if (status == InfoDisplayStatus.Error)
                return _errorBrush;
            return _normalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack should never be called");
        }
    }
}
