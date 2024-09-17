using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SMHaptics
{

    public class SMHOutputDeviceConfig
    {
        public string moduleName;
    }

    public class SMHOutputDevice
    {
        public SMHOutputDeviceConfig config = null;
        public DirectSoundDeviceInfo deviceInfo = null;
        public DirectSoundOut directSoundOut = null;
        public List<ISampleProvider> signalGenerators = new List<ISampleProvider>();
        public MultiplexingSampleProvider sampleProvider;
        public int channelCount = 1;
        public bool enabled = false;

        public void Enable(bool enable)
        {
            enabled = enable;
            if (enabled)
            {
                if (deviceInfo != null)
                {
                    if (directSoundOut == null)
                    {
                        directSoundOut = new DirectSoundOut(deviceInfo.Guid);

                        for(int i = 0; i < channelCount; ++i)
                        {
                            signalGenerators.Add(new SignalGenerator(44100, 1)
                            {
                                Frequency = 0,
                                Type = SignalGeneratorType.Sin,
                                Gain = 0.0f 
                            });
                        }

                        sampleProvider = new MultiplexingSampleProvider(signalGenerators, channelCount);
                        
                        for(int i = 0; i < channelCount; ++i)
                        {
                            sampleProvider.ConnectInputToOutput(i, i);
                        }

                        directSoundOut.Init(sampleProvider);
                    }
                }
            }
            else
            {
                if(directSoundOut != null)
                {
                    directSoundOut.Stop();
                    directSoundOut.Dispose();
                    directSoundOut = null;
                }

            }

        }

        public void Play()
        {
            if(directSoundOut != null)
            {
                if(directSoundOut.PlaybackState != PlaybackState.Playing)
                {
                    directSoundOut.Play();
                }
            }

        }

        public void Stop()
        {
            if (directSoundOut != null)
            {
                if (directSoundOut.PlaybackState != PlaybackState.Stopped)
                    directSoundOut.Stop();
            }
        }

        bool ValidateChannelForPlayback(int channelIndex)
        {
            if (!enabled)
                return false;

            if (channelIndex < 0 || channelIndex >= channelCount)
            {
                return false;
            }

            return true;
        }

        bool ValidateChannelForSilence(int channelIndex)
        {
            if (!enabled)
                return false;

            if (channelIndex < 0 || channelIndex >= channelCount)
            {
                return false;
            }


            return true;
        }


        public void SetFrequency(int channelIndex, double frequency)
        {

            if(ValidateChannelForPlayback(channelIndex))
            {
                SignalGenerator signalGenerator = signalGenerators[channelIndex] as SignalGenerator;

                if (signalGenerator != null)
                {
                    signalGenerator.Frequency = frequency;
                }

            }

        }

        public void SetWaveform(int channelIndex, SignalGeneratorType generatorType)
        {

            if (ValidateChannelForPlayback(channelIndex))
            {
                SignalGenerator signalGenerator = signalGenerators[channelIndex] as SignalGenerator;

                if (signalGenerator != null)
                {
                    signalGenerator.Type = generatorType;
                }
            }
        }


        public void SetGain(int channelIndex, double gain)
        {

            if (ValidateChannelForPlayback(channelIndex))
            {
                SignalGenerator signalGenerator = signalGenerators[channelIndex] as SignalGenerator;

                if (signalGenerator != null)
                {
                    signalGenerator.Gain = gain;
                }
            }
        }

    }

}
