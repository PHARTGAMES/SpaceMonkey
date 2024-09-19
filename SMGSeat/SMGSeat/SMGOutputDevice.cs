using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandMessenger;
using CommandMessenger.Transport;
using CommandMessenger.Transport.Serial;
using System.IO.Ports;

namespace SMGSeat
{
    public class SMGOutputDeviceConfig
    {
        public bool enable;
        public string comPort;
        public int minServoPosition;
        public int maxServoPosition;
    }

    public class SMGOutputDevice
    {
        public enum Commands
        {
            kAcknowledge = 0,
            kError = 1,
            kMessage = 2,
            kSetPosition = 3,
            kSetStandby = 6,
            kSetStop = 7,
            kArduinoReady = 10,
        };

        public SMGOutputDeviceConfig config;

        public void Init(SMGOutputDeviceConfig _config)
        {
            config = _config;
        }

        public void Deinit()
        {

        }

        SerialTransport m_serialTransport;
        CmdMessenger m_cmdMessenger;

        string m_comPort = "COM5";
        public int m_baudRate = 57600;
        const int commandId = 0;


        public string COMPort
        {
            get
            {
                return m_comPort;
            }

            set
            {
                m_comPort = value;

                if (m_cmdMessenger != null)
                {
                    StopCMDMessenger();
                    StartCMDMessenger();
                }
            }
        }

        public void StartCMDMessenger()
        {
            m_serialTransport = new SerialTransport();

            m_serialTransport.CurrentSerialSettings.PortName = m_comPort;
            m_serialTransport.CurrentSerialSettings.BaudRate = m_baudRate;
            m_serialTransport.CurrentSerialSettings.DataBits = 8;
            m_serialTransport.CurrentSerialSettings.StopBits = StopBits.One;
            m_serialTransport.CurrentSerialSettings.Parity = Parity.None;
            m_serialTransport.CurrentSerialSettings.DtrEnable = false;
            m_serialTransport.CurrentSerialSettings.Timeout = 5000;

            m_cmdMessenger = new CmdMessenger((ITransport)(object)m_serialTransport, BoardType.Bit16);
            //m_cmdMessenger.PrintLfCr = true;
            AttachCommandCallbacks();

            m_cmdMessenger.Connect();

        }

        public void StopCMDMessenger()
        {
            if (m_cmdMessenger != null)
            {
                m_cmdMessenger.Disconnect();
                m_cmdMessenger.Dispose();
                m_cmdMessenger = null;
            }

            if (m_serialTransport != null)
            {
                m_serialTransport.Dispose();
                m_serialTransport = null;
            }
        }

        protected void AttachCommandCallbacks()
        {
            m_cmdMessenger.Attach(new CmdMessenger.MessengerCallbackFunction(OnUnknownCommand));
            m_cmdMessenger.Attach((int)Commands.kAcknowledge, new CmdMessenger.MessengerCallbackFunction(OnAcknowledge));
            m_cmdMessenger.Attach((int)Commands.kError, new CmdMessenger.MessengerCallbackFunction(OnError));
            m_cmdMessenger.Attach((int)Commands.kMessage, new CmdMessenger.MessengerCallbackFunction(OnMessage));
            m_cmdMessenger.Attach((int)Commands.kArduinoReady, new CmdMessenger.MessengerCallbackFunction(OnArduinoReady));
            m_cmdMessenger.Attach((int)Commands.kSetStandby, new CmdMessenger.MessengerCallbackFunction(OnStandby));
            m_cmdMessenger.Attach((int)Commands.kSetStop, new CmdMessenger.MessengerCallbackFunction(OnStop));
        }

        private void OnMessage(ReceivedCommand arguments)
        {

        }
        

        private void OnArduinoReady(ReceivedCommand arguments)
        {

        }


        private void OnStandby(ReceivedCommand arguments)
        {

        }

        private void OnStop(ReceivedCommand arguments)
        {

        }

        private void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine("Command without attached callback received: " + ((object)arguments).ToString());
        }

        private void OnAcknowledge(ReceivedCommand arguments)
        {
            Console.WriteLine("Arduino ACK");
        }

        private void OnError(ReceivedCommand arguments)
        {
            Console.WriteLine("Arduino has experienced an error");
        }

        public void SetPositions(List<float> positions)
        {
            int positionRange = config.maxServoPosition - config.minServoPosition;

            SendCommand cmd = new SendCommand((int)Commands.kSetPosition);
            for(int i = 0; i < positions.Count; ++i)
            {
                Int16 pos = (Int16)(config.minServoPosition + (positions[i] * positionRange));

                cmd.AddArgument(pos);
            }
            m_cmdMessenger.SendCommand(cmd, SendQueue.InFrontQueue, ReceiveQueue.Default);
        }

    }
}
