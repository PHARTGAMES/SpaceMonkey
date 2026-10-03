using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SMUtil
{
    public class SMMath
    {
        public static float Map(float v, float inMin, float inMax, float outMin, float outMax, bool clamp = false)
        {
            if (inMax == inMin) return outMin; // or throw
            var t = (v - inMin) / (inMax - inMin); // inverse lerp
            if (clamp)
            {
                if (t < 0f) t = 0f;
                else if (t > 1f) t = 1f;
            }
            return outMin + t * (outMax - outMin); // lerp
        }

        // dir is expected to lie in the XZ plane. Y is ignored.
        public static float YawDegreesFromXZ(Vector3 dir)
        {
            // Optional: normalize if you are not sure it's normalized
            if (dir.X != 0f || dir.Z != 0f)
            {
                float len = (float)Math.Sqrt(dir.X * dir.X + dir.Z * dir.Z);
                dir.X /= len;
                dir.Z /= len;
            }
            else
            {
                return 0f; // or float.NaN
            }

            // 0 deg at +Z; positive clockwise toward +X (right-handed, Y up)
            float radians = (float)Math.Atan2(dir.X, dir.Z);
            float degrees = radians * (180f / (float)Math.PI);
            if (degrees < 0f) degrees += 360f;
            return degrees;
        }

        // Variant: return in [-180, 180)
        public static float YawDegreesSigned(Vector3 dir)
        {
            float d = YawDegreesFromXZ(dir);
            if (d >= 180f) d -= 360f;
            return d;
        }

        public static float SignedYawDelta(float fromDeg, float toDeg)
        {
            // Smallest signed angle from 'fromDeg' to 'toDeg' (degrees), wrap-safe.
            float delta = (toDeg - fromDeg) % 360f; // C# '%' keeps sign of left operand
            if (delta < -180f) delta += 360f;
            else if (delta >= 180f) delta -= 360f;
            return delta;
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0) { var t = min; min = max; max = t; } // optional swap if inputs reversed
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }


        public static float Lerp(float from, float to, float lerp)
        {
            return from + ((to - from) * lerp);
        }

        public static T MoveToward<T>(T current, T target, T maxStep) where T : struct, IComparable<T>
        {
            if (Comparer<T>.Default.Compare(maxStep, default(T)) < 0)
                throw new ArgumentOutOfRangeException(nameof(maxStep), "maxStep must be non-negative.");

            if (Comparer<T>.Default.Compare(target, current) > 0)
            {
                dynamic diff = (dynamic)target - current;
                return Comparer<T>.Default.Compare((T)diff, maxStep) > 0
                    ? (T)((dynamic)current + maxStep)
                    : target;
            }
            else
            {
                dynamic diff = (dynamic)current - target;
                return Comparer<T>.Default.Compare((T)diff, maxStep) > 0
                    ? (T)((dynamic)current - maxStep)
                    : target;
            }
        }


    }
}
