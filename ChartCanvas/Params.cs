using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartCanvas
{
    public enum ImageSaveOption
    {
        Wave = 0,
        SourceSpectrograms = 1,
        SignalSpectrograms = 2
    }

    public enum ChartZoomOption
    {
        XPlus = 0,
        XMinus = 1,
        YPlus = 2,
        YMinus = 3,
        Auto = 4
    }

    public class Param_AM
    {
        public Param_AM(double _moudulateFrequency)
        {
            moudulateFrequency = _moudulateFrequency;
        }

        public double moudulateFrequency { get; set; }
    }
}
