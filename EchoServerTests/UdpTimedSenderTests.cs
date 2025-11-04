using Moq;
using NUnit.Framework;
using EchoServer; // Припускаємо, що інтерфейс там
using System;
using System.Threading;
using System.Net;
using System.Linq;

namespace EchoServerTests
{
    [TestFixture]
    public class UdpTimedSenderTests
    {
        private Mock<IUdpSocket> _udpMock;
        private UdpTimedSender _sender;
        private const string TestHost = "127.0.0.1";
        private const int TestPort = 60000;

        [SetUp]
        public void Setup()
        {
            _udpMock = new Mock<IUdpSocket>();
            _sender = new UdpTimedSender(TestHost, TestPort, _udpMock.Object);
        }

     
        [Test]
        public void ConstructorTest()
        {

            Assert.That(_sender, Is.Not.Null);
        }

        [Test]
        public void StartSending_ThrowsIfAlreadyRunning()
        {
            _sender.StartSending(1000);

            Assert.Throws<InvalidOperationException>(() => _sender.StartSending(1000));

            _sender.StopSending();
        }

        [Test]
        public void StartSending_TimerIsCreated()
        {
            _sender.StartSending(500);

            Assert.DoesNotThrow(() => _sender.StopSending());
        }

        [Test]
        public void SendMessageCallback_SendsData()
        {
            Timer? testTimer = null;

            testTimer = new Timer((state) =>
            {
                _sender.GetType()
                       .GetMethod("SendMessageCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                       .Invoke(_sender, new object?[] { null });
            }, null, Timeout.Infinite, Timeout.Infinite);

            _sender.GetType()
                   .GetMethod("SendMessageCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                   .Invoke(_sender, new object?[] { null });

            _udpMock.Verify(
                udp => udp.Send(
                    It.Is<byte[]>(bytes => bytes.Length > 0),
                    It.Is<int>(len => len > 0),
                    It.IsAny<IPEndPoint>()
                ),
                Times.Once,
                "IUdpSocket.Send має бути викликаний один раз.");

            testTimer.Dispose();
        }
    }
}