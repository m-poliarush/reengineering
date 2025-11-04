using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EchoServer.Tests
{
    [TestFixture]
    public class UdpSocketWrapperTests
    {
        private UdpSocketWrapper _wrapper;
        private IPEndPoint _testEndpoint;
        private byte[] _testData;

        [SetUp]
        public void SetUp()
        {
            _wrapper = new UdpSocketWrapper();
            _testEndpoint = new IPEndPoint(IPAddress.Loopback, 12345);
            _testData = Encoding.ASCII.GetBytes("Test message");
        }

        [TearDown]
        public void TearDown()
        {
            _wrapper.Dispose();
        }

        [Test]
        public void Dispose_ShouldThrowObjectDisposedExceptionOnSubsequentSend()
        {
            _wrapper.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
            {
                _wrapper.Send(_testData, _testData.Length, _testEndpoint);
            }, "Після Dispose метод Send повинен кидати ObjectDisposedException.");
        }
        [Test]
        public void Dispose_ShouldNotThrowException_WhenCalledMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                _wrapper.Dispose();
                _wrapper.Dispose();
            }, "Повторний виклик Dispose не повинен кидати виключення.");
        }
        [Test]
        public void Send_ShouldSendPacket_InIntegrationTest()
        {
            int listenPort = 12346;
            var senderEndpoint = new IPEndPoint(IPAddress.Loopback, listenPort);
            var receiver = new UdpClient(listenPort);
            receiver.Client.ReceiveTimeout = 100;

            int bytesSent = _wrapper.Send(_testData, _testData.Length, senderEndpoint);

            byte[] receivedBytes = Array.Empty<byte>();
            IPEndPoint? remoteIpEndPoint = null;
            try
            {
                receivedBytes = receiver.Receive(ref remoteIpEndPoint);
            }
            catch (SocketException)
            {
            }
            finally
            {
                receiver.Close();
                receiver.Dispose();
            }

            Assert.That(bytesSent, Is.EqualTo(_testData.Length), "Кількість відправлених байтів має збігатися з довжиною даних.");
            Assert.That(receivedBytes.Length, Is.EqualTo(_testData.Length), "Отриманий пакет має мати правильну довжину.");
            Assert.That(receivedBytes, Is.EqualTo(_testData), "Отримані дані мають збігатися з відправленими.");
        }
    }
}