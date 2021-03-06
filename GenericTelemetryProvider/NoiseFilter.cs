﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericTelemetryProvider;

namespace NoiseFilters
{
    public class NoiseFilter
    {
        private float[ ] samples;
        private int maxSampleCount;
        private int liveSampleCount;
        private int currSample = 0;
        private float maxInputDelta = float.MaxValue; //maximum input delta between last and new sample.
        private float currPrediction = 0.0f;

        //higher _maxSampleCount = more smoothing
        public NoiseFilter( int _maxSampleCount, float _maxInputDelta = float.MaxValue )
        {
            maxSampleCount = Math.Max( 1, _maxSampleCount );
            samples = new float[ maxSampleCount ];
            maxInputDelta = _maxInputDelta;
        }

        public float Filter( float sample )
        {
            //early out
            if ( maxSampleCount == 1 )
                return sample;

            if ( maxInputDelta != float.MaxValue && liveSampleCount > 0 )
            {
                float sampleDiff = sample - samples[ currSample ];
                float absSampleDiff = Math.Abs( sampleDiff );
                if ( absSampleDiff > maxInputDelta )
                {
                    float direction = sampleDiff / absSampleDiff;

                    sample = samples[ currSample ] + ( direction * maxInputDelta );

                }
            }

            currSample = ( currSample + 1 ) >= maxSampleCount ? 0 : currSample + 1;
            samples[ currSample ] = sample;

            liveSampleCount = ( liveSampleCount + 1 ) >= maxSampleCount ? maxSampleCount : liveSampleCount + 1;

            //average all samples
            float total = 0.0f;
            for ( int i = 0; i < liveSampleCount; ++i )
            {
                total += samples[ i ];
            }

            float prediction = 0.0f;
            int sampleCounter = 0;
            /*
                        for(int i = currSample; sampleCounter < liveSampleCount; i--)
                        {
                            int prevSample = currSample - 1 < 0 ? liveSampleCount - 1 : currSample - 1;
                            prediction += samples[ currSample ] - samples[ prevSample ];
                            sampleCounter++;
                        }
            */
            for (int i = currSample; sampleCounter < liveSampleCount; i--)
            {
                if (i < 0)
                    i = liveSampleCount - 1;
                int prevSample = i - 1 < 0 ? liveSampleCount - 1 : i - 1;
                prediction += samples[currSample] - samples[prevSample];
                sampleCounter++;
            }

            currPrediction = prediction = ((currPrediction + (prediction / sampleCounter)) * 0.5f);// * 0.75f;
            float smoothedValue = total / liveSampleCount;

            return smoothedValue;// + prediction;
        }

        public void Reset( )
        {
            currSample = 0;
            liveSampleCount = 0;
        }
    }

    public class KalmanFilter
    {
        public float A, H, Q, R, P, x;
        public float initial_P;
        public float initial_x;

        public KalmanFilter( float A, float H, float Q, float R, float initial_P, float initial_x )
        {
            this.A = A;
            this.H = H;
            this.Q = Q;
            this.R = R;
            this.initial_P = this.P = initial_P;
            this.initial_x = this.x = initial_x;
        }

        public float Filter( float input )
        {
            // time update - prediction
            x = A * x;
            P = A * P * A + Q;

            // measurement update - correction
            float K = P * H / ( H * P * H + R );
            x = x + K * ( input - H * x );
            P = ( 1 - K * H ) * P;

            return x;
        }
    }

    public class NestedSmooth
    {
        NoiseFilter filter;
        NestedSmooth nest;
        public int nestCount;
        public int maxSampleCount;
        public float maxInputDelta;
        public float Value = 0.0f;
        public NestedSmooth(int _nestCount, int _maxSampleCount, float _maxInputDelta = float.MaxValue )
        {
            nestCount = _nestCount;
            maxSampleCount = _maxSampleCount;
            maxInputDelta = _maxInputDelta;
            Value = 0.0f;

            if(nestCount != 0)
                nest = new NestedSmooth(nestCount-1, _maxSampleCount, _maxInputDelta);

            filter = new NoiseFilter( _maxSampleCount, _maxInputDelta );
        }

        public float Filter(float value)
        {
            if (nest != null)
            {
                Value = nest.Filter(filter.Filter(value));
                return Value;
            }

            Value = filter.Filter(value);
            return Value;
        }

    }

    public class MedianFilter
    {

        private float[] samples;
        public int maxSampleCount;
        private int liveSampleCount;
        private int currSample = 0;
        private List<float> sorter = new List<float>();

        public MedianFilter(int _maxSampleCount)
        {
            maxSampleCount = Math.Max(3, _maxSampleCount);
            maxSampleCount = (maxSampleCount / 2.0f) == 0.0f ? maxSampleCount + 1 : maxSampleCount;
            samples = new float[maxSampleCount];
        }

        public float Filter(float sample)
        {
            currSample = (currSample + 1) >= maxSampleCount ? 0 : currSample + 1;
            samples[currSample] = sample;

            liveSampleCount = (liveSampleCount + 1) >= maxSampleCount ? maxSampleCount : liveSampleCount + 1;

            return FindMedian();
        }

        float FindMedian()
        {
            if(liveSampleCount < 3)
            {
                return samples[0];
            }

            sorter.Clear();
            for (int i = 0; i < liveSampleCount; ++i)
            {
                sorter.Add(samples[i]);
            }

            sorter.Sort();

            int medianIndex = (sorter.Count / 2);

            return sorter[medianIndex];

        }

    }

    public class KalmanVelocityFilter
    {
        public float A, H, Q, R, P, x;
        public float initial_P;
        public float initial_x;
        public float value;

        KalmanFilter kalman;

        public KalmanVelocityFilter(float A, float H, float Q, float R, float initial_P, float initial_x)
        {
            this.A = A;
            this.H = H;
            this.Q = Q;
            this.R = R;
            this.initial_P = this.P = initial_P;
            this.initial_x = this.x = initial_x;

            kalman = new KalmanFilter(A, H, Q, R, initial_P, initial_x);
        }

        public float Filter(float sample)
        {
            float vel = (sample - value) / GenericProviderBase.dt;

            float newValue = value + kalman.Filter(vel) * GenericProviderBase.dt;

            value = newValue;
            return newValue;
        }

        
    }
}
