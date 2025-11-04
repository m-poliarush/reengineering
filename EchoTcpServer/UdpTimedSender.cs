using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public class UdpTimedSender : DisposableBase
    {
        private readonly string _host;
        private readonly int _port;
        private readonly IUdpSocket _udpClient;
        private Timer? _timer;


        public UdpTimedSender(string host, int port, IUdpSocket udpSocket)
        {
            _host = host;
            _port = port;
            _udpClient = udpSocket;
        }

        public void StartSending(int intervalMilliseconds)
        {
            CheckDisposed();
            if (_timer != null)
                throw new InvalidOperationException("Sender is already running.");

            _timer = new Timer(SendMessageCallback, null, 0, intervalMilliseconds);
        }

        ushort i = 0;

        private void SendMessageCallback(object? state)
        {
            try
            {
                //dummy data
                Random rnd = new Random();
                byte[] samples = new byte[1024];
                rnd.NextBytes(samples);
                i++;

                byte[] msg = (new byte[] { 0x04, 0x84 }).Concat(BitConverter.GetBytes(i)).Concat(samples).ToArray();
                var endpoint = new IPEndPoint(IPAddress.Parse(_host), _port);

                _udpClient.Send(msg, msg.Length, endpoint);
                Console.WriteLine($"Message sent to {_host}:{_port} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public void StopSending()
        {
            _timer?.Dispose();
            _timer = null;
        }
        protected override void DisposeManagedResources()
        {
            StopSending();
            _udpClient.Dispose();
        }
    }
}
