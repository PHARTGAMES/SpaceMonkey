using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseFilters;

namespace GenericTelemetryProvider
{
    public class WashoutFilter : FilterBase
    {
        private float previousInput = 0f;  // Store the previous input value
        private float previousOutput = 0f; // Store the previous output value
        private float alpha;               // Filter coefficient
        private float timeConstant;        // Time constant for the filter

        // Constructor to initialize the filter with a time constant and sample time
        public WashoutFilter()
        {
           
        }

        public void SetParameters(float timeConstant)
        {
            this.timeConstant = timeConstant;
            alpha = timeConstant / (timeConstant + FilterModuleCustom.Instance.deltaTime);  // Recalculate alpha when parameters change
        }

        public float GetTimeConstant()
        {
            return timeConstant;
        }

        // Override the Filter method
        public override float Filter(float currentInput)
        {
            // Apply the washout filter formula
            float output = alpha * (previousOutput + currentInput - previousInput);

            // Update the previous input and output values for the next step
            previousInput = currentInput;
            previousOutput = output;

            return output;
        }
    }

}
