using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Queue.Models;
using Queue.RabbitMQ;
using RabbitMQ.Client;

namespace Queue.Test;

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

        queue.DeleteQueue();
    }

    [Test]
    public async Task Receive_MessageReceived_Successfully()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<RabbitMqMessageQueue<MediaMessage>>>();
        var queue = new RabbitMqMessageQueue<MediaMessage>(QueueConnectionString, "Receive_MessageReceived_Successfully", mockLogger.Object);
        var messageHandled = false;
        var originalMessage = new MediaMessage { Id = "1", Type = "test" };
        //var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(originalMessage));
        //var ea = new BasicDeliverEventArgs { Body = body };
        var messageReceivedEvent = new TaskCompletionSource<bool>();
        MediaMessage? receivedMessage = null;

        // Act
        queue.Receive(async msg =>
        {
            receivedMessage = msg; // Save the received message for comparison
            messageHandled = true;
            messageReceivedEvent.SetResult(true); // Signal that a message has been received
            await Task.CompletedTask;
        });

        // Simulate sending a message to the queue
        await queue.SendAsync(originalMessage, EventTypes.Added);

        // Wait for the message to be received or for a timeout
        if (!await messageReceivedEvent.Task.TimeoutAfter(TimeSpan.FromSeconds(5))) // Adjust the timeout as needed
        {
            Assert.Fail("Timeout waiting for message to be received.");
        }

        queue.DeleteQueue();

        // Assert
        Assert.That(messageHandled, Is.True);
        Assert.That(receivedMessage, Is.Not.Null);
        Assert.That(receivedMessage.Id, Is.EqualTo(originalMessage.Id));
        Assert.That(receivedMessage.Type, Is.EqualTo(originalMessage.Type));
    }
}

public static class TaskExtensions
{
    public static async Task<bool> TimeoutAfter(this Task task, TimeSpan timeout)
    {
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
        {
            await task;  // Await the task to propagate any exception.
            return true;
        }

        return false;
    }
}