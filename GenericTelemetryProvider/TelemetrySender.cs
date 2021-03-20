using System;
using System.Net;
using System.Net.Sockets;

namespace GenericTelemetryProvider
{
    public class TelemetrySender : IDisposable
    {
        private IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);
        private UdpClient udpClient;

        public void StartSending(string ip, int port)
        {
            try
            {
                if (udpClient != null)
                {
                    StopSending();
                }
                udpClient = new UdpClient();
                udpClient.Connect(ip, port);
                disposed = false;
            }
            catch
            {
                udpClient = null;
            }
        }

        public bool IsConnected()
        {
            return udpClient != null;
        }

        public void StopSending()
        {
            if (udpClient != null)
                udpClient.Close();
        }

        public void SendAsync(byte[] data)
        {
            udpClient.SendAsync(data, data.Length);
        }

        private bool disposed = false;

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    udpClient.Close();
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
