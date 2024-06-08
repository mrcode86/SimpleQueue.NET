using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Queue.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Queue.Test
{
    [TestFixture]
    public class RabbitMqMessageQueueTests : TestBase
    {
        [Test]
        public void Send_MessageSent_Successfully()
        {
            // Arrange
            var mockChannel = new Mock<IModel>();
            var mockConnection = new Mock<IConnection>();
            mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);
            var mockLogger = new Mock<ILogger<RabbitMqMessageQueue<MediaMessage>>>();
            var queue = new RabbitMqMessageQueue<MediaMessage>(QueueConnectionString, "Send_MessageSent_Successfully", mockLogger.Object);
            var message = new MediaMessage();

            // Act & Assert
            Assert.DoesNotThrow(() => queue.Send(message, EventTypes.Added));

            queue.CloseConnection();
        }

        [Test]
        public void Receive_MessageReceived_Successfully()
        {
            // Arrange
            var mockChannel = new Mock<IModel>();
            var mockConnection = new Mock<IConnection>();
            mockConnection.Setup(c => c.CreateModel()).Returns(mockChannel.Object);
            var mockLogger = new Mock<ILogger<RabbitMqMessageQueue<MediaMessage>>>();
            var queue = new RabbitMqMessageQueue<MediaMessage>(QueueConnectionString, "Receive_MessageReceived_Successfully", mockLogger.Object);
            var messageHandled = false;
            var originalMessage = new MediaMessage { Id = "1", Type = "test" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(originalMessage));
            var ea = new BasicDeliverEventArgs { Body = body };
            var messageReceivedEvent = new AutoResetEvent(false);
            MediaMessage? receivedMessage = null;

            // Act
            queue.Receive(msg =>
            {
                receivedMessage = msg; // Save the received message for comparison
                messageHandled = true;
                messageReceivedEvent.Set(); // Signal that a message has been received
            });

            // Simulate sending a message to the queue
            queue.Send(originalMessage, EventTypes.Added);

            // Wait for the message to be received or for a timeout
            if (!messageReceivedEvent.WaitOne(TimeSpan.FromSeconds(5))) // Adjust the timeout as needed
            {
                Assert.Fail("Timeout waiting for message to be received.");
            }

            queue.CloseConnection();

            // Assert
            Assert.IsTrue(messageHandled);
            Assert.IsNotNull(receivedMessage);
            Assert.That(receivedMessage.Id, Is.EqualTo(originalMessage.Id));
            Assert.That(receivedMessage.Type, Is.EqualTo(originalMessage.Type));
        }
    }
}