using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public interface IUdpSocket : IDisposable
    {
        int Send(byte[] dgram, int bytes, IPEndPoint endPoint);
    }
}
