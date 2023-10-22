using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using System.Threading;
using System.Diagnostics;


namespace XInputFFB
{

    public class DIInputDetectionResult
    {
        public string m_deviceID;
        public string m_objectID;
        public int m_index;
        public int m_delta;

        public string Identifier { get { return $"{m_deviceID}, {m_objectID}, {m_index}"; } }
    }


    public class DIFFEffect
    {
        DIDevice m_device;
        EffectInfo m_effectInfo;
        public DIFFEffect(DIDevice a_device, EffectInfo a_effectInfo)
        {
            m_device = a_device;
            m_effectInfo = a_effectInfo;

            Console.WriteLine("Create FFEffect: " + ID);

            m_device.AddFFEffect(this);
        }
        public string ID { get { return m_effectInfo.Name; } }

    }

    public class DIObject
    {
        public DIDevice m_device;
        public DeviceObjectInstance m_objectInstance;
        public DIObject(DIDevice a_device, DeviceObjectInstance a_objectInstance)
        {
            m_device = a_device;
            m_objectInstance = a_objectInstance;

        }
    }

    public class DIButton : DIObject
    {
        public DIButton(DIDevice a_device, DeviceObjectInstance a_objectInstance) : base(a_device, a_objectInstance)
        {
            Console.WriteLine("Create Button: " + ID);
        }
        public string ID { get { return m_objectInstance.Name; } }

    }

    public class DIAxis : DIObject
    {
        public DIAxis(DIDevice a_device, DeviceObjectInstance a_objectInstance) : base(a_device, a_objectInstance)
        {
            Console.WriteLine("Create Axis: " + ID);
        }
        public string ID { get { return m_objectInstance.Name; } }
    }

    public class DIDevice
    {
        DeviceInstance m_deviceInstance;
        DInputDeviceManager m_manager;
        List<DIObject> m_objects = new List<DIObject>();
        List<DIFFEffect> m_ffEffects = new List<DIFFEffect>();

        public JoystickState m_currentState;
        public JoystickState m_lastState;

        bool m_acquired = false;
        public DIDevice(DInputDeviceManager a_manager, DeviceInstance a_deviceInstance)
        {
            m_manager = a_manager;
            m_deviceInstance = a_deviceInstance;

            m_joystick = new Joystick(m_manager.m_directInput, m_deviceInstance.InstanceGuid);

            Console.WriteLine("Create Device: " + ID);

            Enumerate();

            m_manager.AddDevice(this);
        }

        public Joystick m_joystick;

        public string ID
        {
            get
            {
                return m_deviceInstance.InstanceName;
            }
        }

        public void Acquire()
        {
            if (m_acquired)
                return;

            m_acquired = true;
            m_joystick.Acquire();
        }

        public void Unacquire()
        {
            if (!m_acquired)
                return;

            m_joystick.Unacquire();
            m_acquired = false;
        }

        public void Enumerate()
        {
            // Enumerate all buttons
            var buttons = m_joystick.GetObjects(DeviceObjectTypeFlags.Button);
            foreach(DeviceObjectInstance button in buttons)
            {
                DIButton newButton = new DIButton(this, button);
            }

            // Enumerate all axes
            var axes = m_joystick.GetObjects(DeviceObjectTypeFlags.Axis);
            foreach (DeviceObjectInstance axis in axes)
            {
                DIAxis newAxis = new DIAxis(this, axis);
            }

            var ffEffects = m_joystick.GetEffects();
            foreach(EffectInfo ffEffect in ffEffects)
            {
                DIFFEffect newFFEffect = new DIFFEffect(this, ffEffect);
            }

        }


        public void AddObject(DIObject a_object)
        {
            m_objects.Add(a_object);
        }

        public void AddFFEffect(DIFFEffect a_effect)
        {
            m_ffEffects.Add(a_effect);
        }

        public JoystickState PollState(bool a_reset = false)
        {
            m_joystick.Poll();
            m_lastState = m_currentState;
            m_currentState = m_joystick.GetCurrentState();
            if (a_reset)
                m_lastState = m_currentState;

            return m_currentState;
        }

        public List<DIInputDetectionResult> DetectInputChanges()
        {
            List<DIInputDetectionResult> inputChanges = new List<DIInputDetectionResult>();

            bool[] lastButtons = m_lastState.Buttons;
            bool[] currButtons = m_currentState.Buttons;

            for(int i = 0; i < lastButtons.Length; ++i)
            {
                if(lastButtons[i] != currButtons[i])
                {
                    DIInputDetectionResult change = new DIInputDetectionResult();
                    change.m_deviceID = ID;
                    change.m_objectID = "Buttons";
                    change.m_index = i;
                    change.m_delta = 1;
                    inputChanges.Add(change);
                }
            }

            int[] lastPointOfViewControllers = m_lastState.PointOfViewControllers;
            int[] currentPointOfViewControllers = m_currentState.PointOfViewControllers;

            for(int i = 0; i < lastPointOfViewControllers.Length; ++i)
            {
                if(lastPointOfViewControllers[i] != currentPointOfViewControllers[i])
                {
                    DIInputDetectionResult change = new DIInputDetectionResult();
                    change.m_deviceID = ID;
                    change.m_objectID = "PointOfViewControllers";
                    change.m_index = i;
                    change.m_delta = currentPointOfViewControllers[i] - lastPointOfViewControllers[i];
                    inputChanges.Add(change);
                }
            }

            int[] lastSliders = m_lastState.Sliders;
            int[] currentSliders = m_currentState.Sliders;

            for (int i = 0; i < lastSliders.Length; ++i)
            {
                if (lastSliders[i] != currentSliders[i])
                {
                    DIInputDetectionResult change = new DIInputDetectionResult();
                    change.m_deviceID = ID;
                    change.m_objectID = "Sliders";
                    change.m_index = i;
                    change.m_delta = currentSliders[i] - lastSliders[i];
                    inputChanges.Add(change);
                }
            }

            lastSliders = m_lastState.VelocitySliders;
            currentSliders = m_currentState.VelocitySliders;

            for (int i = 0; i < lastSliders.Length; ++i)
            {
                if (lastSliders[i] != currentSliders[i])
                {
                    DIInputDetectionResult change = new DIInputDetectionResult();
                    change.m_deviceID = ID;
                    change.m_objectID = "VelocitySliders";
                    change.m_index = i;
                    change.m_delta = currentSliders[i] - lastSliders[i];
                    inputChanges.Add(change);
                }
            }

            lastSliders = m_lastState.AccelerationSliders;
            currentSliders = m_currentState.AccelerationSliders;

            for (int i = 0; i < lastSliders.Length; ++i)
            {
                if (lastSliders[i] != currentSliders[i])
                {
                    DIInputDetectionResult change = new DIInputDetectionResult();
                    change.m_deviceID = ID;
                    change.m_objectID = "AccelerationSliders";
                    change.m_index = i;
                    change.m_delta = currentSliders[i] - lastSliders[i];
                    inputChanges.Add(change);
                }
            }

            lastSliders = m_lastState.ForceSliders;
            currentSliders = m_currentState.ForceSliders;

            for (int i = 0; i < lastSliders.Length; ++i)
            {
                if (lastSliders[i] != currentSliders[i])
                {
                    DIInputDetectionResult change = new DIInputDetectionResult();
                    change.m_deviceID = ID;
                    change.m_objectID = "ForceSliders";
                    change.m_index = i;
                    change.m_delta = currentSliders[i] - lastSliders[i];
                    inputChanges.Add(change);
                }
            }

            if (m_currentState.X != m_lastState.X)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "X";
                change.m_index = -1;
                change.m_delta = m_currentState.X - m_lastState.X;
                inputChanges.Add(change);
            }

            if (m_currentState.Y != m_lastState.Y)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "Y";
                change.m_index = -1;
                change.m_delta = m_currentState.Y - m_lastState.Y;
                inputChanges.Add(change);
            }

            if (m_currentState.Z != m_lastState.Z)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "Z";
                change.m_index = -1;
                change.m_delta = m_currentState.Z - m_lastState.Z;
                inputChanges.Add(change);
            }

            if (m_currentState.RotationX != m_lastState.RotationX)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "RotationX";
                change.m_index = -1;
                change.m_delta = m_currentState.RotationX - m_lastState.RotationX;
                inputChanges.Add(change);
            }

            if (m_currentState.RotationY != m_lastState.RotationY)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "RotationY";
                change.m_index = -1;
                change.m_delta = m_currentState.RotationY - m_lastState.RotationY;
                inputChanges.Add(change);
            }

            if (m_currentState.RotationZ != m_lastState.RotationZ)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "RotationZ";
                change.m_index = -1;
                change.m_delta = m_currentState.RotationZ - m_lastState.RotationZ;
                inputChanges.Add(change);
            }

            if (m_currentState.AccelerationX != m_lastState.AccelerationX)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AccelerationX";
                change.m_index = -1;
                change.m_delta = m_currentState.AccelerationX - m_lastState.AccelerationX;
                inputChanges.Add(change);
            }

            if (m_currentState.AccelerationY != m_lastState.AccelerationY)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AccelerationY";
                change.m_index = -1;
                change.m_delta = m_currentState.AccelerationY - m_lastState.AccelerationY;
                inputChanges.Add(change);
            }

            if (m_currentState.AccelerationZ != m_lastState.AccelerationZ)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AccelerationZ";
                change.m_index = -1;
                change.m_delta = m_currentState.AccelerationZ - m_lastState.AccelerationZ;
                inputChanges.Add(change);
            }

            if (m_currentState.AngularAccelerationX != m_lastState.AngularAccelerationX)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AngularAccelerationX";
                change.m_index = -1;
                change.m_delta = m_currentState.AngularAccelerationX - m_lastState.AngularAccelerationX;
                inputChanges.Add(change);
            }

            if (m_currentState.AngularAccelerationY != m_lastState.AngularAccelerationY)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AngularAccelerationY";
                change.m_index = -1;
                change.m_delta = m_currentState.AngularAccelerationY - m_lastState.AngularAccelerationY;
                inputChanges.Add(change);
            }

            if (m_currentState.AngularAccelerationZ != m_lastState.AngularAccelerationZ)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AngularAccelerationZ";
                change.m_index = -1;
                change.m_delta = m_currentState.AngularAccelerationZ - m_lastState.AngularAccelerationZ;
                inputChanges.Add(change);
            }

            if (m_currentState.AngularVelocityX != m_lastState.AngularVelocityX)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AngularVelocityX";
                change.m_index = -1;
                change.m_delta = m_currentState.AngularVelocityX - m_lastState.AngularVelocityX;
                inputChanges.Add(change);
            }

            if (m_currentState.AngularVelocityY != m_lastState.AngularVelocityY)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AngularVelocityY";
                change.m_index = -1;
                change.m_delta = m_currentState.AngularVelocityY - m_lastState.AngularVelocityY;
                inputChanges.Add(change);
            }

            if (m_currentState.AngularVelocityZ != m_lastState.AngularVelocityZ)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "AngularVelocityZ";
                change.m_index = -1;
                change.m_delta = m_currentState.AngularVelocityZ - m_lastState.AngularVelocityZ;
                inputChanges.Add(change);
            }

            if (m_currentState.ForceX != m_lastState.ForceX)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "ForceX";
                change.m_index = -1;
                change.m_delta = m_currentState.ForceX - m_lastState.ForceX;
                inputChanges.Add(change);
            }

            if (m_currentState.ForceY != m_lastState.ForceY)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "ForceY";
                change.m_index = -1;
                change.m_delta = m_currentState.ForceY - m_lastState.ForceY;
                inputChanges.Add(change);
            }

            if (m_currentState.ForceZ != m_lastState.ForceZ)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "ForceZ";
                change.m_index = -1;
                change.m_delta = m_currentState.ForceZ - m_lastState.ForceZ;
                inputChanges.Add(change);
            }

            if (m_currentState.TorqueX != m_lastState.TorqueX)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "TorqueX";
                change.m_index = -1;
                change.m_delta = m_currentState.TorqueX - m_lastState.TorqueX;
                inputChanges.Add(change);
            }

            if (m_currentState.TorqueY != m_lastState.TorqueY)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "TorqueY";
                change.m_index = -1;
                change.m_delta = m_currentState.TorqueY - m_lastState.TorqueY;
                inputChanges.Add(change);
            }

            if (m_currentState.TorqueZ != m_lastState.TorqueZ)
            {
                DIInputDetectionResult change = new DIInputDetectionResult();
                change.m_deviceID = ID;
                change.m_objectID = "TorqueZ";
                change.m_index = -1;
                change.m_delta = m_currentState.TorqueZ - m_lastState.TorqueZ;
                inputChanges.Add(change);
            }


            return inputChanges;
        }
    }

    public class DInputDeviceManager
    {
        public static DInputDeviceManager Instance;

        public DirectInput m_directInput;
        List<DIDevice> m_devices = new List<DIDevice>();

        public DInputDeviceManager()
        {
            Instance = this;

            m_directInput = new DirectInput();

            EnumerateDevices();
        }
        
        public void EnumerateDevices()
        {
            m_devices = new List<DIDevice>();

            var devices = m_directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices);

            foreach (DeviceInstance deviceInstance in devices)
            {
                DIDevice newDevice = new DIDevice(this, deviceInstance);
            }
        }

        public void AddDevice(DIDevice a_device)
        {
            m_devices.Add(a_device);
        }

        public DIDevice GetDevice(string a_id)
        {
            return m_devices.Find((x) => x.ID == a_id);
        }

        public void DetectInput(Action<List<DIInputDetectionResult>> a_callback)
        {
            Console.WriteLine("Detecting Input Start...");
            Thread x = new Thread(new ParameterizedThreadStart((a_detectionCallback) =>
            {
                foreach (DIDevice device in m_devices)
                {
                    device.Acquire();
                    device.PollState(true);
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                List<DIInputDetectionResult> results = new List<DIInputDetectionResult>();
                do
                {
                    foreach (DIDevice device in m_devices)
                    {
                        device.PollState();

                        results.AddRange(device.DetectInputChanges());
                    }

                    //sort most frequent result to the top
                    results = results
                    .GroupBy(item => item.Identifier)
                    .OrderByDescending(group => group.Count())
                    .SelectMany(group => group)
                    .ToList();


                    bool sameResultPresent = results
                    .Take(6) // Take the top
                    .Select(item => item.Identifier)
                    .Distinct() // Get distinct Identifier values from the top
                    .Count() == 1;

                    if (sameResultPresent || (results.Count > 3 && sw.Elapsed.TotalSeconds > 5))
                    {
                        break;
                    }

                    Thread.Sleep(16);
                }
                while (true);

                foreach (DIDevice device in m_devices)
                {
                    device.Unacquire();
                }

                ((Action<List<DIInputDetectionResult>>)a_detectionCallback).Invoke(results);

                Console.WriteLine("Detecting Input End");
            }));
            x.IsBackground = true;
            x.Start(a_callback);

        }


    }
}
