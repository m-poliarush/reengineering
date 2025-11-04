using Moq;
using NUnit.Framework;
using EchoServer;
using System;
using System.Threading;
using System.Net;
using System.Linq;
using System.Reflection;

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

        [TearDown]
        public void Teardown()
        {
            _sender?.Dispose();
            _sender = null!;
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
            var callbackMethod = typeof(UdpTimedSender).GetMethod(
                "SendMessageCallback",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            callbackMethod!.Invoke(_sender, new object?[] { null });

            _udpMock.Verify(
                udp => udp.Send(
                    It.Is<byte[]>(bytes => bytes.Length == 1028),
                    It.Is<int>(len => len == 1028),
                    It.IsAny<IPEndPoint>()
                ),
                Times.Once,
                "IUdpSocket.Send має бути викликаний один раз."
            );

        }

        [Test]
        public void Dispose_CallsUdpClientDispose()
        {
            _sender.Dispose();

            _udpMock.Verify(udp => udp.Dispose(), Times.Once, "Dispose має бути викликаний для IUdpSocket.");
        }
    }
}