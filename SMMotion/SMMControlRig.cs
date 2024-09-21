using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using System.Numerics;
using Newtonsoft.Json;

namespace SMMotion
{

    public class SMControlRigConfig
    {
        public bool enabled; //is it on?
        public float rigWidth; //how wide is the rig
        public float rigLength; //how long is the rig
        public float actuatorVerticalOffset; //actuator vertical offset from center
        public float actuatorStroke; //how far the actuators move
        public float actuatorLength; //how far from actuator center point to floor
        public Vector3 headLocalOffset; //rig relative head local position
        public float accelerationScale; //scalar for acceleration
        public float maxAcceleration; //maximum acceleration value



        public delegate void ConfigChangedHandler(SMControlRigConfig config, string fieldName);

        public event ConfigChangedHandler ConfigChanged;


        protected void OnConfigChanged(string fieldName)
        {
            ConfigChanged?.Invoke(this, fieldName);
        }

        [JsonIgnore]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnConfigChanged("enabled");
                }
            }
        }

        [JsonIgnore]
        public float RigWidth
        {
            get => rigWidth;
            set
            {
                if (rigWidth != value)
                {
                    rigWidth = value;
                    OnConfigChanged("rigWidth");
                }
            }
        }

        [JsonIgnore]
        public float RigLength
        {
            get => rigLength;
            set
            {
                if (rigLength != value)
                {
                    rigLength = value;
                    OnConfigChanged("rigLength");
                }
            }
        }
        [JsonIgnore]
        public float ActuatorVerticalOffset
        {
            get => actuatorVerticalOffset;
            set
            {
                if (actuatorVerticalOffset != value)
                {
                    actuatorVerticalOffset = value;
                    OnConfigChanged("actuatorVerticalOffset");
                }
            }
        }
        [JsonIgnore]
        public float ActuatorStroke
        {
            get => actuatorStroke;
            set
            {
                if (actuatorStroke != value)
                {
                    actuatorStroke = value;
                    OnConfigChanged("actuatorStroke");
                }
            }
        }
        [JsonIgnore]
        public float ActuatorLength
        {
            get => actuatorLength;
            set
            {
                if (actuatorLength != value)
                {
                    actuatorLength = value;
                    OnConfigChanged("actuatorLength");
                }
            }
        }
        [JsonIgnore]
        public Vector3 HeadLocalOffset
        {
            get => headLocalOffset;
            set
            {
                if (headLocalOffset != value)
                {
                    headLocalOffset = value;
                    OnConfigChanged("headLocalOffset");
                }
            }
        }
        [JsonIgnore]
        public float AccelerationScale
        {
            get => accelerationScale;
            set
            {
                if (accelerationScale != value)
                {
                    accelerationScale = value;
                    OnConfigChanged("accelerationScale");
                }
            }
        }
        [JsonIgnore]
        public float MaxAcceleration
        {
            get => maxAcceleration;
            set
            {
                if (maxAcceleration != value)
                {
                    maxAcceleration = value;
                    OnConfigChanged("maxAcceleration");
                }
            }
        }


    }

    public class SMMControlRig
    {

        protected SMMControlState controlStateOut;

        public SMControlRigConfig config;

        public virtual void Init(SMControlRigConfig _config)
        {
            config = _config;
            config.ConfigChanged += ConfigChanged;

            controlStateOut = new SMMControlState();

        }

        public virtual void Cleanup()
        {

        }

        public virtual SMMControlState Update(CMCustomUDPData dataIn, float dt)
        {
            return controlStateOut;
        }

        public virtual void ConfigChanged(SMControlRigConfig config, string fieldName)
        {
            switch (fieldName)
            {
                case "enabled":
                    {
                        break;
                    }
                case "rigWidth":
                    {
                        break;
                    }
                case "rigLength":
                    {
                        break;
                    }
                case "actuatorVerticalOffset":
                    {
                        break;
                    }
                case "actuatorStroke":
                    {
                        break;
                    }
                case "actuatorLength":
                    {
                        break;
                    }
                case "headLocalOffset":
                    {
                        break;
                    }
                case "accelerationScale":
                    {
                        break;
                    }
                case "maxAcceleration":
                    {
                        break;
                    }
            }
        }
    }
}
