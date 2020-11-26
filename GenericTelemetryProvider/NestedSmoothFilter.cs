using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseFilters;

namespace GenericTelemetryProvider
{
    public class NestedSmoothFilter : FilterBase
    {
       NestedSmooth filter;

        public override float Filter(float value)
        {
            return filter.Filter(value);
        }

        public void SetParameters(int nestCount, int sampleCount, float maxDelta)
        {
            filter = new NestedSmooth(nestCount, sampleCount, maxDelta);
        }

        public int GetNestCount()
        {
            return filter.nestCount;
        }

        public int GetSampleCount()
        {
            return filter.maxSampleCount;
        }

        public float GetMaxDelta()
        {
            return filter.maxInputDelta;
        }

    }
}
