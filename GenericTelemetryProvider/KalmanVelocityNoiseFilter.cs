using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseFilters;

namespace GenericTelemetryProvider
{
    public class KalmanVelocityNoiseFilter : FilterBase
    {
        KalmanVelocityFilter filter;

        public override float Filter(float value)
        {
            return filter.Filter(value);
        }

        public void SetParameters(float A, float H, float Q, float R, float initial_P, float initial_x)
        {
            filter = new KalmanVelocityFilter(A, H, Q, R, initial_P, initial_x);
        }

        public float GetA()
        {
            return filter.A;
        }

        public float GetH()
        {
            return filter.H;
        }

        public float GetQ()
        {
            return filter.Q;
        }

        public float GetR()
        {
            return filter.R;
        }

        public float GetP()
        {
            return filter.initial_P;
        }

        public float GetX()
        {
            return filter.initial_x;
        }

    }
}
