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

        public float maxWheelAngle = 35.0f;
        public float constantForceAngleCurve = 1.0f;

        public float minConstantForce = 1000.0f;
        public float maxConstantForce = 10000.0f;
        public float constantForceSpeedCurve = 1.0f;

        public float minDampForce = 1000.0f;
        public float maxDampForce = 10000.0f;
        public float dampForceSpeedCurve = 1.0f;

        private void SetFloat(ref float field, float value, string key)
        {
            if (field != value)
            {
                field = value;
                OnConfigChanged(key);
            }
        }

        [JsonIgnore]
        public float ConstantForceAngleCurve
        {
            get => constantForceAngleCurve;
            set => SetFloat(ref constantForceAngleCurve, value, "constantForceAngleCurve");
        }


        [JsonIgnore]
        public float MaxWheelAngle
        {
            get => maxWheelAngle;
            set => SetFloat(ref maxWheelAngle, value, "maxWheelAngle");
        }

        [JsonIgnore]
        public float MinConstantForce
        {
            get => minConstantForce;
            set => SetFloat(ref minConstantForce, value, "minConstantForce");
        }

        [JsonIgnore]
        public float MaxConstantForce
        {
            get => maxConstantForce;
            set => SetFloat(ref maxConstantForce, value, "maxConstantForce");
        }


        [JsonIgnore]
        public float ConstantForceSpeedCurve
        {
            get => constantForceSpeedCurve;
            set => SetFloat(ref constantForceSpeedCurve, value, "constantForceSpeedCurve");
        }

        [JsonIgnore]
        public float MinDampForce
        {
            get => minDampForce;
            set => SetFloat(ref minDampForce, value, "minDampForce");
        }

        [JsonIgnore]
        public float MaxDampForce
        {
            get => maxDampForce;
            set => SetFloat(ref maxDampForce, value, "maxDampForce");
        }

        [JsonIgnore]
        public float DampForceSpeedCurve
        {
            get => dampForceSpeedCurve;
            set => SetFloat(ref dampForceSpeedCurve, value, "dampForceSpeedCurve");
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

                    suspensionOffset = (suspensionDiffFl + suspensionDiffFr);

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
            float maxWorldVelocity = 10.0f;
            float maxWheelAngleDiff = 35.0f;


            if(worldVelocityMag < minWorldVelocity)
            {
                outputs.ffb_wheel_steer_constant = 0.0f;
            }
            else
            {
                float velocityScalar = Math.Min(worldVelocityMag / maxWorldVelocity, 1.0f);

                float localWheelAngle = (float)inputs.steering_input * wheelSteerEffectConfig.maxWheelAngle;

                Vector3 vehicleWorldForwardXZ = new Vector3((float)inputs.world_dir_fwd_x, 0.0f, (float)inputs.world_dir_fwd_z);
                vehicleWorldForwardXZ = Vector3.Normalize(vehicleWorldForwardXZ);

                Vector3 worldVelXZNorm = Vector3.Normalize(worldVelocityXZ);

                float vehicleVelocityWorldYaw = SMMath.YawDegreesFromXZ(worldVelXZNorm);
                float wheelWorldYaw = (SMMath.YawDegreesFromXZ(vehicleWorldForwardXZ) + localWheelAngle) % 360.0f;

                float angleDiff = SMMath.Clamp(SMMath.SignedYawDelta(vehicleVelocityWorldYaw, wheelWorldYaw), -maxWheelAngleDiff, maxWheelAngleDiff);

                float constantForceFromAngle = (angleDiff / maxWheelAngleDiff);

                float constantForceFromSuspension = 1.0f + SMMath.Clamp(suspensionOffset, -1.0f, 1.0f);

                float constantForceScalar = (float)Math.Pow((double)velocityScalar, (double)wheelSteerEffectConfig.constantForceSpeedCurve) * (float)Math.Pow((double)constantForceFromAngle, (double)wheelSteerEffectConfig.constantForceAngleCurve);

                //tweak by suspension
                constantForceScalar *= constantForceFromSuspension;

                outputs.ffb_wheel_steer_constant = SMMath.Map(constantForceScalar, 0.0f, 1.0f, wheelSteerEffectConfig.minConstantForce, wheelSteerEffectConfig.maxConstantForce);

                float dampForceScalar = (float)Math.Pow((double)velocityScalar, (double)wheelSteerEffectConfig.dampForceSpeedCurve);

                outputs.ffb_wheel_steer_damper = SMMath.Map(dampForceScalar, 0.0f, 1.0f, wheelSteerEffectConfig.minDampForce, wheelSteerEffectConfig.maxDampForce);

//                Debug.WriteLine($"suspensionOffset = {suspensionOffset}");
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
