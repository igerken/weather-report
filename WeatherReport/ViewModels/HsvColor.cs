using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherReport.ViewModels
{
    public class HsvColor : Tuple<float, float, float>
    {
        public float Hue { get { return Item1; } }
        public float Saturation { get { return Item2; } }
        public float Value { get { return Item3; } }

        public HsvColor(float hue, float sat, float val)
            : base(hue, sat, val)
        {
        }
    }
}
