using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;

namespace SMHaptics
{

    public class SMHEngineEffectConfig : SMHEffectConfig
    {
        public double minFrequency;
        public double maxFrequency;


        [JsonIgnore]
        public double MinFrequency
        {
            get => minFrequency;
            set
            {
                if (minFrequency != value)
                {
                    minFrequency = value;
                    OnConfigChanged("minFrequency");
                }
            }
        }


        [JsonIgnore]
        public double MaxFrequency
        {
            get => maxFrequency;
            set
            {
                if (maxFrequency != value)
                {
                    maxFrequency = value;
                    OnConfigChanged("maxFrequency");
                }
            }
        }


    }

    public class SMHEngineEffect : SMHEffect
    {
        SMHEngineEffectConfig engineEffectConfig = null;

        public override void Init(SMHEffectConfig config)
        {
            base.Init(config);

            engineEffectConfig = config as SMHEngineEffectConfig;
        }

        public override void Update(CMCustomUDPData inputs)
        {
            base.Update(inputs);

            if (!effectConfig.enabled)
                return;

            if (outputDevice == null)
                return;

            double playbackFrequency = 60;

            double rpmRange = (float)inputs.max_rpm - (float)inputs.idle_rpm;
            double engineRate = (float)inputs.engine_rate;

            double rateNorm = Math.Max(0.0, Math.Min(1.0, engineRate / rpmRange));

            double frequencyRange = engineEffectConfig.maxFrequency - engineEffectConfig.minFrequency;

            playbackFrequency = engineEffectConfig.minFrequency + (frequencyRange * rateNorm);

            outputDevice.SetFrequency(effectConfig.outputChannelIndex, playbackFrequency);
            outputDevice.SetWaveform(effectConfig.outputChannelIndex, (SignalGeneratorType)effectConfig.waveform);
            outputDevice.SetGain(effectConfig.outputChannelIndex, effectConfig.gain);
            outputDevice.Play();

        }


        public override void ConfigChanged(SMHEffectConfig config, string fieldName)
        {
            base.ConfigChanged(config, fieldName);

            switch (fieldName)
            {
                case "minFrequency":
                    {
                        break;
                    }
                case "maxFrequency":
                    {
                        break;
                    }
            }
        }

    }
}
