using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseFilters;

namespace GenericTelemetryProvider
{
    public class MedianFilterWrapper : FilterBase
    {
       MedianFilter filter;

        public override float Filter(float value)
        {
            return filter.Filter(value);
        }

        public void SetParameters(int sampleCount)
        {
            filter = new MedianFilter(sampleCount);
        }

        public int GetSampleCount()
        {
            return filter.maxSampleCount;
        }
    }
}
