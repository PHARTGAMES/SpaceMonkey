using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandMessenger;
using CommandMessenger.Transport;
using CommandMessenger.Transport.Serial;
using System.IO.Ports;


namespace XInputFFB
{
	public enum XInputControl 
	{
		BUTTON_LOGO = 0,
		BUTTON_A = 1,
		BUTTON_B = 2,
		BUTTON_X = 3,
		BUTTON_Y = 4,
		BUTTON_LB = 5,
		BUTTON_RB = 6,
		BUTTON_BACK = 7,
		BUTTON_START = 8,
		BUTTON_L3 = 9,
		BUTTON_R3 = 10,
		DPAD_UP = 11,
		DPAD_DOWN = 12,
		DPAD_LEFT = 13,
		DPAD_RIGHT = 14,
		TRIGGER_LEFT = 15,
		TRIGGER_RIGHT = 16,
		JOY_LEFT = 17,
		JOY_RIGHT = 18,
	};

	public class XInputFFBCom
    {
		static XInputFFBCom m_instance;
		public static XInputFFBCom Instance
        {
			get
            {
				if(m_instance == null)
                {
					m_instance = new XInputFFBCom();
                }
				return m_instance;
            }
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

				if(m_cmdMessenger != null)
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
			if(m_cmdMessenger != null)
            {
				m_cmdMessenger.Disconnect();
				m_cmdMessenger.Dispose();
				m_cmdMessenger = null;
            }

			if(m_serialTransport != null)
            {
				m_serialTransport.Dispose();
				m_serialTransport = null;
            }
        }

		protected void AttachCommandCallbacks()
        {
			m_cmdMessenger.Attach(new CmdMessenger.MessengerCallbackFunction(OnUnknownCommand));
			m_cmdMessenger.Attach(0, new CmdMessenger.MessengerCallbackFunction(OnAcknowledge));
			m_cmdMessenger.Attach(1, new CmdMessenger.MessengerCallbackFunction(OnError));
			m_cmdMessenger.Attach(2, new CmdMessenger.MessengerCallbackFunction(OnMessage));
		}
		private void OnMessage(ReceivedCommand arguments)
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

		public void SendControlStateButton(XInputControl a_control, bool a_pressed)
        {
			if (m_cmdMessenger == null)
				return;

			SendCommand cmd = new SendCommand(commandId, (Int16)a_control);
			cmd.AddArgument(a_pressed);
			m_cmdMessenger.SendCommand(cmd, SendQueue.InFrontQueue, ReceiveQueue.Default);
		}

		public void SendControlStateStick(XInputControl a_control, Int32 a_xAxis, Int32 a_yAxis)
		{
			if (m_cmdMessenger == null)
				return;

			SendCommand cmd = new SendCommand(commandId, (Int16)a_control);
			cmd.AddArgument(a_xAxis);
			cmd.AddArgument(a_yAxis);
			m_cmdMessenger.SendCommand(cmd, SendQueue.InFrontQueue, ReceiveQueue.Default);

		}

		public void SendControlStateTrigger(XInputControl a_control, Int32 a_axis)
		{
			if (m_cmdMessenger == null)
				return;

			SendCommand cmd = new SendCommand(commandId, (Int16)a_control);
			cmd.AddArgument(a_axis);
			m_cmdMessenger.SendCommand(cmd, SendQueue.InFrontQueue, ReceiveQueue.Default);
		}


	}
}
