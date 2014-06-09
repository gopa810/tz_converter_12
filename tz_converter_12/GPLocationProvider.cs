using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tz_converter_12
{
    public class GPLocationProvider
    {
        public GPLocation getLocation(double d)
        {
            return null;
        }

        public void setDefaultLocation(GPLocation loc)
        {
        }
    }

    public class GPLocation
    {
        public double getTimeZoneOffsetHours()
        {
            return 0;
        }

        public GPTimeZoneOld getTimeZone()
        {
            return null;
        }
    }

    public class GPJulianTime
    {
        public GPJulianTime()
        {
        }

        public GPJulianTime(double time, double of)
        {
        }

        public double getGreenwichJulianDay()
        {
            return 0;
        }
    }
}
