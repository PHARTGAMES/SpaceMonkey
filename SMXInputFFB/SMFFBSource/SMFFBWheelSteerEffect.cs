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
using System.Diagnostics.Eventing.Reader;


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

        public float minVibrationFrequency = 1000.0f;
        public float maxVibrationFrequency = 10000.0f;
        public float minVibrationGain = 1000.0f;
        public float maxVibrationGain = 10000.0f;
        public float vibrationSpeedCurve = 1.0f;
        public float verticalAccelScale = 0.5f;
        public float verticalAccelMax = 0.25f;




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

        [JsonIgnore]
        public float MinVibrationFrequency
        {
            get => minVibrationFrequency;
            set => SetFloat(ref minVibrationFrequency, value, "minVibrationFreq");
        }

        [JsonIgnore]
        public float MaxVibrationFrequency
        {
            get => maxVibrationFrequency;
            set => SetFloat(ref maxVibrationFrequency, value, "maxVibrationFrequency");
        }

        [JsonIgnore]
        public float MinVibrationGain
        {
            get => minVibrationGain;
            set => SetFloat(ref minVibrationGain, value, "minVibrationGain");
        }

        [JsonIgnore]
        public float MaxVibrationGain
        {
            get => maxVibrationGain;
            set => SetFloat(ref maxVibrationGain, value, "maxVibrationGain");
        }

        [JsonIgnore]
        public float VibrationSpeedCurve
        {
            get => vibrationSpeedCurve;
            set => SetFloat(ref vibrationSpeedCurve, value, "vibrationSpeedCurve");
        }

        [JsonIgnore]
        public float VerticalAccelScale
        {
            get => verticalAccelScale;
            set => SetFloat(ref verticalAccelScale, value, "verticalAccelScale");
        }

        [JsonIgnore]
        public float VerticalAccelMax
        {
            get => verticalAccelMax;
            set => SetFloat(ref verticalAccelMax, value, "verticalAccelMax");
        }
    }



    public class SMFFBWheelSteerEffect : SMFFBEffect
    {
        SMFFBWheelSteerEffectConfig wheelSteerEffectConfig = null;
        CMCustomUDPData lastFrame = null;
        float verticalAccelOffset = 0.0f;

        public override void Init(SMFFBEffectConfig config)
        {
            base.Init(config);

            wheelSteerEffectConfig = config as SMFFBWheelSteerEffectConfig;

        }

        float ShapeTanh(float x, float softness)
        {
            // softness < 1 makes center softer
            // e.g. softness = 2 to 5
            return (float)Math.Tanh(x * softness) / (float)Math.Tanh(softness);
        }

        float ShapeBlend(float x, float amount)
        {
            // amount: 0 = linear, 1 = fully cubic
            float cubic = x * x * x;
            return x + (cubic - x) * amount;
        }

        float SoftZone(float x, float zone)
        {
            float ax = Math.Abs(x);
            if (ax >= zone)
                return x;

            float t = ax / zone;
            float shaped = t * t * (3.0f - 2.0f * t); // smoothstep
            return Math.Sign(x) * shaped * zone;
        }

        float ShapeCubic(float x)
        {
            // x in [-1, 1]
            return x * x * x;
        }

        float ShapePower(float x, float power)
        {
            float ax = Math.Abs(x);
            return Math.Sign(x) * (float)Math.Pow(ax, power);
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

                    float suspensionDiffFr = suspensionFr - (float)lastFrame.suspension_position_fr;
                    float suspensionDiffFl = suspensionFl - (float)lastFrame.suspension_position_fl;

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
                    outputs.ffb_wheel_steer_vibration_gain = 0.0f;
                    outputs.ffb_wheel_steer_vibration_freq = 0.0f;
                    outputs.ffb_wheel_steer_damper = 0.0f;
                    outputs.ffb_wheel_steer_spring = 0.0f;
                    outputs.ffb_wheel_steer_friction = 0.0f;
                    return;
               }
            }

            Vector3 worldVelocityXZ = new Vector3((float)inputs.world_velocity_x, 0.0f, (float)inputs.world_velocity_z);

            float worldVelocityMag = worldVelocityXZ.Length();
            float minWorldVelocity = 0.001f;
            float maxWorldVelocity = 10.0f;
            float maxWheelAngleDiff = 35.0f;

            float deltaTime = (float)inputs.total_time - (float)lastFrame.total_time;


            //if(worldVelocityMag < minWorldVelocity)
            //{
            //    outputs.ffb_wheel_steer_constant = 0.0f;
            //    outputs.ffb_wheel_steer_vibration_gain = 0.0f;
            //    outputs.ffb_wheel_steer_vibration_freq = 0.0f;
            //    outputs.ffb_wheel_steer_damper = 0.0f;
            //    outputs.ffb_wheel_steer_spring = 0.0f;
            //}
            //else
            {
                float velocityScalar = Math.Min(worldVelocityMag / maxWorldVelocity, 1.0f);

                float localWheelAngle = (float)inputs.steering_input * wheelSteerEffectConfig.maxWheelAngle;

                float noiseFreq = 0.5f;
                //noise steer vibration
                float steerNoise = Perlin2D.Noise(new Vector2((float)inputs.position_x, (float)inputs.position_z) * noiseFreq) * velocityScalar;

                Vector3 vehicleWorldForwardXZ = new Vector3((float)inputs.world_dir_fwd_x, 0.0f, (float)inputs.world_dir_fwd_z);
                vehicleWorldForwardXZ = Vector3.Normalize(vehicleWorldForwardXZ);


                verticalAccelOffset = SMMath.Lerp(verticalAccelOffset, 0.0f, 0.5f * deltaTime);

                verticalAccelOffset = SMMath.Clamp(verticalAccelOffset + (SMMath.Clamp((float)inputs.gforce_vertical / wheelSteerEffectConfig.verticalAccelMax, -1.0f, 1.0f) * wheelSteerEffectConfig.verticalAccelScale * deltaTime), -0.25f, 0.25f);


//                verticalAccelOffset = ShapePower(SMMath.Clamp(verticalAccelOffset + (SMMath.Clamp((float)inputs.gforce_vertical / wheelSteerEffectConfig.verticalAccelMax, -1.0f, 1.0f) * wheelSteerEffectConfig.verticalAccelScale), -0.25f, 0.25f), 2.0f);
                float constantForceFromSuspensionScalar = 1.0f + (SMMath.Clamp(suspensionOffset * 2.0f, -4.0f, 4.0f));


                float constantForceScalar = 0.0f;
                float constantForceOutput = 0.0f;
                float vibrationAngleScalar = 0.0f;
                float constantForceFromAngle = 0.0f;
                float velDir = 1.0f;
                if (worldVelocityMag >= minWorldVelocity)
                {
                    Vector3 worldVelXZNorm = Vector3.Normalize(worldVelocityXZ);

                    float forwardDotVel = Vector3.Dot(vehicleWorldForwardXZ, worldVelXZNorm);
                    if (forwardDotVel < 0.0f)
                    {
                        worldVelXZNorm = -worldVelXZNorm;
                        velDir = -1.0f;
                    }

                    float vehicleVelocityWorldYaw = SMMath.YawDegreesFromXZ(worldVelXZNorm);
                    float wheelWorldYaw = (SMMath.YawDegreesFromXZ(vehicleWorldForwardXZ) + localWheelAngle) % 360.0f;

                    float angleDiff = SMMath.Clamp(SMMath.SignedYawDelta(vehicleVelocityWorldYaw, wheelWorldYaw), -maxWheelAngleDiff, maxWheelAngleDiff) * velDir;

                    constantForceFromAngle = (angleDiff / maxWheelAngleDiff);

                    constantForceScalar = (float)Math.Pow((double)velocityScalar, (double)wheelSteerEffectConfig.constantForceSpeedCurve) * ((float)Math.Pow((double)Math.Abs(constantForceFromAngle), (double)wheelSteerEffectConfig.constantForceAngleCurve) * Math.Sign(constantForceFromAngle));
                    //constantForceScalar = velocityScalar * constantForceFromAngle;

//                    vibrationAngleScalar = 1.0f + (SMMath.Clamp(Math.Abs(constantForceFromAngle), 0.0f, 1.0f) * 0.5f);
                    vibrationAngleScalar = 1.0f + (SMMath.Clamp(Math.Abs(constantForceScalar), 0.0f, 1.0f) * 0.5f);

                    //tweak by suspension
                    //                constantForceScalar *= constantForceFromSuspensionScalar;

                    //tweak by steer noise
                    //                constantForceScalar += constantForceScalar * 0.25f * steerNoise;

                    //tweak constant force by vertical acceleration
                    constantForceScalar *= 1.0f + Math.Abs(verticalAccelOffset);

//                    constantForceScalar = ShapeTanh(constantForceScalar, 0.1f);
//                    constantForceScalar = ShapeBlend(constantForceScalar, 0.8f);
//                    constantForceScalar = SoftZone(constantForceScalar, 0.1f);
//                    constantForceScalar = ShapeCubic(constantForceScalar);
                    constantForceScalar = ShapePower(constantForceScalar, 2.0f);


                    //constant force
                    constantForceOutput = Math.Sign(constantForceScalar) * SMMath.Map(Math.Abs(constantForceScalar), 0.0f, 1.0f, wheelSteerEffectConfig.minConstantForce, wheelSteerEffectConfig.maxConstantForce);
                }

                //outputs.ffb_wheel_steer_constant = SMMath.Lerp((float)outputs.ffb_wheel_steer_constant, constantForceOutput, Math.Min(1.0f, deltaTime * 5.0f));
                //outputs.ffb_wheel_steer_constant = SMMath.MoveToward((float)outputs.ffb_wheel_steer_constant, constantForceOutput, 250.0f);
                outputs.ffb_wheel_steer_constant = constantForceOutput;

                //vibration
                float vibrationScalar = (float)Math.Pow((double)velocityScalar, (double)wheelSteerEffectConfig.vibrationSpeedCurve) * vibrationAngleScalar;

                //tweak vibration by vertical accel
                vibrationScalar += verticalAccelOffset;

                //tweak vibration by steer angle
                float vibrationGainScalar = vibrationScalar;

                outputs.ffb_wheel_steer_vibration_gain = SMMath.Map(vibrationGainScalar, 0.0f, 1.0f, wheelSteerEffectConfig.minVibrationGain, wheelSteerEffectConfig.maxVibrationGain);

                outputs.ffb_wheel_steer_vibration_freq = SMMath.Map(vibrationScalar * constantForceFromSuspensionScalar, 0.0f, 1.0f, wheelSteerEffectConfig.minVibrationFrequency, wheelSteerEffectConfig.maxVibrationFrequency);

                float steerNoiseFreqRange = 40.0f;

                outputs.ffb_wheel_steer_vibration_freq = (float)outputs.ffb_wheel_steer_vibration_freq + (steerNoise * steerNoiseFreqRange);

                float steerNoiseGainRange = 60.0f;
                outputs.ffb_wheel_steer_vibration_gain = (float)outputs.ffb_wheel_steer_vibration_gain + (steerNoise * steerNoiseGainRange);


                //FIXME: debug
//                outputs.ffb_wheel_steer_constant = 0.0f;
                outputs.ffb_wheel_steer_damper = 2000.0f;
                outputs.ffb_wheel_steer_spring = 0.0f;
                //                outputs.ffb_wheel_steer_vibration_gain = 0.0f;
                outputs.ffb_wheel_steer_friction = 0.0f;

                //float springRange = 0.7f;
                //float springScalar = 1.0f - Math.Min(1.0f, Math.Abs((float)inputs.steering_input) / springRange) * velocityScalar;
                //float springMagnitude = 4000.0f;
                //outputs.ffb_wheel_steer_spring = springScalar * springMagnitude;

                float frictionMin = 50.0f;
                float frictionMax = 2000.0f;

                //float frictionScalar = (1.0f - Math.Abs(velocityScalar)) * Math.Min(1.0f, Math.Abs(constantForceFromAngle));
                //frictionScalar = SMMath.Map(frictionScalar, 0.0f, 1.0f, frictionMin, frictionMax);
//                outputs.ffb_wheel_steer_friction = frictionScalar;

                float frictionScalar = 1.0f-Math.Min(1.0f, Math.Abs(constantForceScalar));
                frictionScalar = SMMath.Map(frictionScalar, 0.0f, 1.0f, frictionMin, frictionMax);
                outputs.ffb_wheel_steer_friction = frictionScalar;
//                outputs.ffb_wheel_steer_friction = 500.0f;

                float damperMin = 1000.0f;
                float damperMax = 3000.0f;

                float damperScalar = 1.0f - Math.Min(1.0f, Math.Abs(constantForceScalar));
                damperScalar = SMMath.Map(damperScalar, 0.0f, 1.0f, damperMin, damperMax);
                outputs.ffb_wheel_steer_damper = damperScalar;



                //FIXME: debug
                //                outputs.ffb_wheel_steer_friction = 0.0f;

                //float frictionRange = 0.7f;
                //float frictionScalar = 1.0f - Math.Min(1.0f, Math.Abs((float)inputs.steering_input) / frictionRange);
                //float frictionMagnitude = 3000.0f;
                //outputs.ffb_wheel_steer_friction = frictionScalar * frictionMagnitude;



                //                Debug.WriteLine($"suspensionOffset = {suspensionOffset}");
                //Debug.WriteLine($"vehicleVelocityWorldYaw = {vehicleVelocityWorldYaw}");
                //Debug.WriteLine($"wheelWorldYaw = {wheelWorldYaw}");
                //Debug.WriteLine($"localWheelAngle = {localWheelAngle}");
                //                Debug.WriteLine($"angleDiff = {angleDiff}");
                //                Debug.WriteLine($"outputs.ffb_wheel_steer_vibration_gain = {outputs.ffb_wheel_steer_vibration_gain}");
                //Debug.WriteLine($"steerNoise = {steerNoise}");
                //                Debug.WriteLine($"inputs.gforce_vertical = {inputs.gforce_vertical}");

                //                Debug.WriteLine($"constantForceScalar = {constantForceScalar}");
                //                Debug.WriteLine($"frictionScalar = {frictionScalar}");
                //                Debug.WriteLine($"damperScalar = {damperScalar}");
//                                Debug.WriteLine($"verticalAccelOffset = {verticalAccelOffset}");

            }

            //            Debug.WriteLine($"ffb_wheel_steer_constant = {outputs.ffb_wheel_steer_constant}");
            //                        Debug.WriteLine($"ffb_wheel_steer_damper = {outputs.ffb_wheel_steer_damper}");

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
