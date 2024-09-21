using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using System.Numerics;

namespace SMMotion
{
    public class SMMControlRig_3D_4A : SMMControlRig
    {
        const int ACTUATOR_COUNT = 4;

        public enum ActuatorID
        {
            FrontRight,
            RearRight,
            RearLeft,
            FrontLeft,
        }

        Vector3 headRestPositionWorld;
        Vector3 headPositionWorld;
        Matrix4x4 rigOrientation;

        Vector3[] actuatorPositionsWorld = new Vector3[4];
        Vector3[] actuatorPositionsLocal = new Vector3[4];
        float[] actuatorLengthWorld = new float[4];


        public override void Init(SMControlRigConfig _config)
        {
            base.Init(_config);

            headPositionWorld = headRestPositionWorld = config.headLocalOffset;

            controlStateOut.actuatorPositions = new float[ACTUATOR_COUNT];
            for (int i = 0; i <ACTUATOR_COUNT; ++i)
            {
                controlStateOut.actuatorPositions[i] = 0;
            }

        }

        public override void Cleanup()
        {

        }

        public override SMMControlState Update(CMCustomUDPData dataIn, float dt)
        {
            if (!config.enabled)
                return null;

//            Console.WriteLine("-------------------------------------------------------------");

            //FIXME: apply rotation telem


            //apply acceleration telem
            Vector3 accelOffsetLocal = new Vector3((float)dataIn.gforce_lateral, (float)dataIn.gforce_vertical, (float)dataIn.gforce_longitudinal);

//            Console.WriteLine($"accelOffsetLocal: {accelOffsetLocal.ToString()}");

            Vector3 accelOffsetWorld = accelOffsetLocal * config.accelerationScale;

            if(accelOffsetWorld.Length() > float.Epsilon)
            {
                accelOffsetWorld = Vector3.Normalize(accelOffsetWorld) * Math.Min(config.maxAcceleration, accelOffsetWorld.Length());
            }

            headPositionWorld = headRestPositionWorld + accelOffsetWorld;

            //world rig pivot
            Vector3 rigPivotWorld = new Vector3(headRestPositionWorld.X, 0, headRestPositionWorld.Z);

            Vector3 headToRigPivot = Vector3.Normalize(rigPivotWorld - headPositionWorld);

            if(headToRigPivot.Length() == 0)
            {
                headToRigPivot = new Vector3(0.0f, -1.0f, 0.0f);
            }

            //project headPositionWorld along headToRigPivot by head position vertical offset
            rigPivotWorld = headPositionWorld + (headToRigPivot * config.headLocalOffset.Y);

            //calc orthogonal vectors
            Vector3 rigFwd = new Vector3(0, 0, 1);
            Vector3 rigUp = -headToRigPivot;
            Vector3 rigRht = Vector3.Cross(rigUp, rigFwd);
            rigFwd = Vector3.Cross(rigRht, rigUp);

            float halfWidth = config.rigWidth * 0.5f;
            float halfLength = config.rigLength * 0.5f;

            Vector3 seatOffset = new Vector3(config.headLocalOffset.X, 0.0f, config.headLocalOffset.Z);

            rigOrientation.M11 = rigRht.X;
            rigOrientation.M12 = rigRht.Y;
            rigOrientation.M13 = rigRht.Z;

            rigOrientation.M21 = rigUp.X;
            rigOrientation.M22 = rigUp.Y;
            rigOrientation.M23 = rigUp.Z;

            rigOrientation.M31 = rigFwd.X;
            rigOrientation.M32 = rigFwd.Y;
            rigOrientation.M33 = rigFwd.Z;

            rigOrientation.M41 = rigPivotWorld.X;
            rigOrientation.M42 = rigPivotWorld.Y;
            rigOrientation.M43 = rigPivotWorld.Z;
            rigOrientation.M44 = 1.0f;

            //calc actuator positions relative to 0,0,0 and offset by negative seat offset
            actuatorPositionsLocal[(int)ActuatorID.FrontLeft] = new Vector3(-halfWidth, config.actuatorVerticalOffset, halfLength) - seatOffset;
            actuatorPositionsLocal[(int)ActuatorID.FrontRight] = new Vector3(halfWidth, config.actuatorVerticalOffset, halfLength) - seatOffset;
            actuatorPositionsLocal[(int)ActuatorID.RearLeft] = new Vector3(-halfWidth, config.actuatorVerticalOffset, -halfLength) - seatOffset;
            actuatorPositionsLocal[(int)ActuatorID.RearRight] = new Vector3(halfWidth, config.actuatorVerticalOffset, -halfLength) - seatOffset;

            float halfActuatorStroke = config.actuatorStroke * 0.5f;

            //ground sits at actuatorLength + actuatorVerticalOffset below origin
            Vector3 planePoint = new Vector3(0.0f, -(config.actuatorLength + config.actuatorVerticalOffset), 0.0f);

//            Console.WriteLine($"headPositionWorld: {headPositionWorld.ToString()}");

            for (int i = 0; i < actuatorPositionsLocal.Length; ++i)
            {
                //transform local actuator positions into world space
                actuatorPositionsWorld[i] = Vector3.Transform(actuatorPositionsLocal[i], rigOrientation);

                //project actuator against ground plane to calc length of actuator
                Vector3 intersectionPoint = new Vector3();
                RayIntersectsPlane(actuatorPositionsWorld[i], -rigUp, new Vector3(0.0f, 1.0f, 0.0f), planePoint, out intersectionPoint, out actuatorLengthWorld[i]);

//                Console.WriteLine($"{((ActuatorID)i).ToString()} length : {actuatorLengthWorld[i]}");

                //calc final actuator position
                controlStateOut.actuatorPositions[i] = -Math.Max(-halfActuatorStroke, Math.Min(halfActuatorStroke, actuatorLengthWorld[i] - (config.actuatorLength + config.actuatorVerticalOffset))) / config.actuatorStroke;

//                Console.WriteLine($"{((ActuatorID)i).ToString()} pos : {controlStateOut.actuatorPositions[i]}");
            }

            return controlStateOut;
        }


        public static bool RayIntersectsPlane(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planeNormal, Vector3 planePoint, out Vector3 intersectionPoint, out float t)
        {
            intersectionPoint = Vector3.Zero; // Initialize the output variable
            t = 0;

            // Calculate the denominator (dot product of ray direction and plane normal)
            float denominator = Vector3.Dot(rayDirection, planeNormal);

            // If the denominator is 0, the ray is parallel to the plane
            if (Math.Abs(denominator) < 1e-6)
            {
                return false; // No intersection, the ray is parallel to the plane
            }

            // Calculate the numerator (dot product of vector from ray origin to plane point and plane normal)
            float numerator = Vector3.Dot(planePoint - rayOrigin, planeNormal);

            // Calculate t (the distance along the ray direction)
            t = numerator / denominator;

            // If t < 0, the intersection is behind the ray's origin, which may not be considered valid
            if (t < 0)
            {
                return false; // Intersection is behind the ray origin
            }

            // Calculate the intersection point using the ray equation: P(t) = o + t * d
            intersectionPoint = rayOrigin + t * rayDirection;

            return true; // Intersection found
        }


        
    }

}
