using System.Net.Sockets;
using System.Text;


namespace EchoServer.Tests
{
    [TestFixture]
    public class EchoServerIntegrationTests
    {
        private EchoServer _server;
        private const int TestPort = 5001;

        [SetUp]
        public async Task SetUp()
        {
            _server = new EchoServer(TestPort);
            _ = Task.Run(() => _server.StartAsync());

            await Task.Delay(100);
        }

        [TearDown]
        public void TearDown()
        {
            _server.Stop();
        }

        [Test]
        public async Task Server_ShouldEchoBackClientMessage()
        {
            string message = "Hello Echo";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            byte[] readBuffer = new byte[buffer.Length];
            int bytesRead;

            using (var client = new TcpClient())
            {

                await client.ConnectAsync("127.0.0.1", TestPort);

                using (var stream = client.GetStream())
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);

                    bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                }


                Assert.That(bytesRead, Is.EqualTo(buffer.Length), "Кількість отриманих байт не збігається.");

                string responseMessage = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                Assert.That(responseMessage, Is.EqualTo(message), "Повідомлення, що повернулося, не збігається.");
            }
        }
    }
}