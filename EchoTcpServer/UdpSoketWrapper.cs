using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public class UdpSocketWrapper : IUdpSocket
    {
        private readonly UdpClient _udpClient;
        private bool _disposed = false;

        public UdpSocketWrapper()
        {
            _udpClient = new UdpClient();
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(UdpSocketWrapper));
            }
            return _udpClient.Send(dgram, bytes, endPoint);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _udpClient.Dispose();
                }

                _disposed = true;
            }
        }
        }
}
