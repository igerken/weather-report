using System;

using WeatherReport.MVVM;

namespace WeatherReport.ViewModels
{
    public class ScrewViewModel : ObservableObject
    {
        public double _size;

        public double _slitX1;
        public double _slitY1;
        public double _slitX2;
        public double _slitY2;

        public double SlitX1
        {
            get { return _slitX1; }
            set
            {
                if (_slitX1 != value)
                {
                    _slitX1 = value;
                    RaisePropertyChanged("SlitX1");
                }
            }
        }

        public double SlitY1
        {
            get { return _slitY1; }
            set
            {
                if (_slitY1 != value)
                {
                    _slitY1 = value;
                    RaisePropertyChanged("SlitY1");
                }
            }
        }

        public double SlitX2
        {
            get { return _slitX2; }
            set
            {
                if (_slitX2 != value)
                {
                    _slitX2 = value;
                    RaisePropertyChanged("SlitX2");
                }
            }
        }

        public double SlitY2
        {
            get { return _slitY2; }
            set
            {
                if (_slitY2 != value)
                {
                    _slitY2 = value;
                    RaisePropertyChanged("SlitY2");
                }
            }
        }

        public ScrewViewModel(double size)
        {
            _size = size;
        }

        public void SetSize(double size)
        {
            _size = size;
        }

        public void RecalculateSlitCoordinates(double slitDirection)
        {
            SlitX1 = 0.5 * _size * (1.0 + Math.Sin(slitDirection));
            SlitY1 = 0.5 * _size * (1.0 - Math.Cos(slitDirection));

            SlitX2 = 0.5 * _size * (1.0 + Math.Sin(slitDirection + Math.PI));
            SlitY2 = 0.5 * _size * (1.0 - Math.Cos(slitDirection + Math.PI));
        }
    }
}
