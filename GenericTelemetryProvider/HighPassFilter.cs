using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseFilters;

namespace GenericTelemetryProvider
{
    public class HighPassFilter : FilterBase
    {
        public float _previousValue = 0.0f;
        public float _cutOffFrequency = 10.0f;
        public float _timeConstant = 1.0f / 60.0f;

        public override float Filter(float value)
        {
            float alpha = _cutOffFrequency / (_cutOffFrequency + _timeConstant);
            float filteredValue = alpha * (_previousValue + value - _previousValue);
            _previousValue = filteredValue;
            return filteredValue;
        }

        public void SetParameters(float cutOffFrequency, float timeConstant)
        {
            _cutOffFrequency = cutOffFrequency;
            _timeConstant = timeConstant;
        }


    }
}
