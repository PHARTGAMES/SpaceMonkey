using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using Newtonsoft.Json;
using System.Numerics;
using SMUtil;
using System.Diagnostics;


namespace SMFFBSource
{

    public class SMFFBWheelSteerEffectConfig : SMFFBEffectConfig
    {
        public float maxWheelAngle = 35.0f; // localWheelAngle = maxWheelAngle * steeringInput
        public float maxWorldVelocity = 10.0f;
        public float constantForceScale = 7000.0f;
        public float maxWheelAngleDiff = 35.0f;
        public float suspensionConstantForceContrib = 10.0f;
        public float velocityConstantForceContrib = 1.0f;


        [JsonIgnore]
        public float MaxWheelAngle
        {
            get => MaxWheelAngle;
            set
            {
                if (MaxWheelAngle != value)
                {
                    MaxWheelAngle = value;
                    OnConfigChanged("maxWheelAngle");
                }
            }
        }

    }

    public class SMFFBWheelSteerEffect : SMFFBEffect
    {
        SMFFBWheelSteerEffectConfig wheelSteerEffectConfig = null;
        CMCustomUDPData lastFrame = null;

        public override void Init(SMFFBEffectConfig config)
        {
            base.Init(config);

            wheelSteerEffectConfig = config as SMFFBWheelSteerEffectConfig;
        }

        public override void Update(CMCustomUDPData inputs, CMCustomUDPData outputs)
        {
            base.Update(inputs, outputs);

            if (lastFrame == null)
            {
                lastFrame = new CMCustomUDPData();
                lastFrame.Init(inputs.formatFilename);
            }


            if (!effectConfig.enabled)
                return;

            float suspensionOffset = 0.0f;

            switch((CMCustomUDPData.VehicleType)(float)inputs.vehicle_type)
            {
                case CMCustomUDPData.VehicleType.Car:
                {
                    float suspensionFr = (float)inputs.suspension_position_fr;
                    float suspensionFl = (float)inputs.suspension_position_fl;

                    float suspensionDiffFl = suspensionFl - (float)lastFrame.suspension_position_fl;
                    float suspensionDiffFr = suspensionFr - (float)lastFrame.suspension_position_fr;

                    suspensionOffset = (suspensionDiffFl + suspensionDiffFr);// / 2.0f;

                    break;
                }

                case CMCustomUDPData.VehicleType.Bike:
                {
                    suspensionOffset = (float)inputs.suspension_position_fr;

                    break;
                }

                case CMCustomUDPData.VehicleType.Pedestrian:
                {
                    outputs.ffb_wheel_steer_constant = 0.0f;
                    outputs.ffb_wheel_steer_collision = 0.0f;
                    outputs.ffb_wheel_steer_damper = 0.0f;
                    return;
               }
            }

            Vector3 worldVelocityXZ = new Vector3((float)inputs.world_velocity_x, 0.0f, (float)inputs.world_velocity_z);

            float worldVelocityMag = worldVelocityXZ.Length();
            float minWorldVelocity = 0.001f;

            if(worldVelocityMag < minWorldVelocity)
            {
                outputs.ffb_wheel_steer_constant = 0.0f;
            }
            else
            {
                float velocityScalar = Math.Min(worldVelocityMag / wheelSteerEffectConfig.maxWorldVelocity, 1.0f);

                float constantForceFromVelocity = (float)Math.Pow(wheelSteerEffectConfig.velocityConstantForceContrib * velocityScalar, 1.5f);

                float localWheelAngle = (float)inputs.steering_input * wheelSteerEffectConfig.maxWheelAngle;

                Vector3 vehicleWorldForwardXZ = new Vector3((float)inputs.world_dir_fwd_x, 0.0f, (float)inputs.world_dir_fwd_z);
                vehicleWorldForwardXZ = Vector3.Normalize(vehicleWorldForwardXZ);

                Vector3 worldVelXZNorm = Vector3.Normalize(worldVelocityXZ);

                float vehicleVelocityWorldYaw = SMMath.YawDegreesFromXZ(worldVelXZNorm);
                float wheelWorldYaw = (SMMath.YawDegreesFromXZ(vehicleWorldForwardXZ) + localWheelAngle) % 360.0f;

                float angleDiff = SMMath.Clamp(SMMath.SignedYawDelta(vehicleVelocityWorldYaw, wheelWorldYaw), -wheelSteerEffectConfig.maxWheelAngleDiff, wheelSteerEffectConfig.maxWheelAngleDiff);

                float constantForceFromAngle = (angleDiff / wheelSteerEffectConfig.maxWheelAngleDiff);

                float constantForceFromSuspension = 1.0f + SMMath.Clamp(suspensionOffset * wheelSteerEffectConfig.suspensionConstantForceContrib, -1.0f, 1.0f);

                outputs.ffb_wheel_steer_constant = constantForceFromAngle * constantForceFromSuspension * constantForceFromVelocity * wheelSteerEffectConfig.constantForceScale;


                Debug.WriteLine($"suspensionOffset = {suspensionOffset}");
                //Debug.WriteLine($"vehicleVelocityWorldYaw = {vehicleVelocityWorldYaw}");
                //Debug.WriteLine($"wheelWorldYaw = {wheelWorldYaw}");
                //Debug.WriteLine($"localWheelAngle = {localWheelAngle}");
//                Debug.WriteLine($"angleDiff = {angleDiff}");


            }

//            Debug.WriteLine($"ffb_wheel_steer_constant = {outputs.ffb_wheel_steer_constant}");

            lastFrame.Copy(inputs, false);
        }


        public override void ConfigChanged(SMFFBEffectConfig config, string fieldName)
        {
            base.ConfigChanged(config, fieldName);

            switch (fieldName)
            {
                case "maxWheelAngle":
                {
                    break;
                }
            }
        }

    }
}
