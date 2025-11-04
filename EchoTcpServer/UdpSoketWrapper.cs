using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public class UdpSocketWrapper : DisposableBase, IUdpSocket
    {
        private readonly UdpClient _udpClient;

        public UdpSocketWrapper()
        {
            _udpClient = new UdpClient();
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
        {
            CheckDisposed();
            return _udpClient.Send(dgram, bytes, endPoint);
        }
        protected override void DisposeManagedResources()
        {
            _udpClient.Dispose();
        }
        
    }
}
