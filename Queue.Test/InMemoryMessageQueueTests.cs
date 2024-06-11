using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Queue.Models;

namespace Queue.Test;

public class InMemoryMessageQueueTests
{
    private InMemoryMessageQueue<MediaMessage> _queue;
    private Mock<ILogger<InMemoryMessageQueue<MediaMessage>>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<InMemoryMessageQueue<MediaMessage>>>();
        _queue = new InMemoryMessageQueue<MediaMessage>(_loggerMock.Object);
    }

    [Test]
    public async Task SendAsync_ShouldEnqueueMessage()
    {
        // Arrange
        var message = new MediaMessage { Id = "1", Type = "Test" };

        // Act
        await _queue.SendAsync(message, EventTypes.Added);

        // Assert
        var messagesHandled = 0;
        _queue.Receive(msg =>
        {
            Assert.That(message.Id, Is.EqualTo(msg.Id));
            Assert.That(message.Type, Is.EqualTo(msg.Type));
            messagesHandled++;
            return Task.CompletedTask;
        });

        await Task.Delay(200); // Allow some time for the message to be processed
        Assert.That(messagesHandled, Is.EqualTo(1));
    }

    [Test]
    public void Receive_ShouldProcessMessages()
    {
        // Arrange
        var message = new MediaMessage { Id = "1", Type = "Test" };
        _queue.Send(message, EventTypes.Added);
        var messagesHandled = 0;

        // Act
        _queue.Receive(async msg =>
        {
            Assert.That(message.Id, Is.EqualTo(msg.Id));
            Assert.That(message.Type, Is.EqualTo(msg.Type));
            messagesHandled++;
            await Task.CompletedTask;
        });

        // Allow some time for the message to be processed
        Task.Delay(200).Wait();

        // Assert
        Assert.That(messagesHandled, Is.EqualTo(1));
    }

    [Test]
    public void DeleteQueue_ShouldClearAllMessages()
    {
        // Arrange
        var message = new MediaMessage { Id = "1", Type = "Test" };
        _queue.Send(message, EventTypes.Added);

        // Act
        _queue.DeleteQueue();

        // Assert
        var messagesHandled = 0;
        _queue.Receive(async msg =>
        {
            Assert.That(message.Id, Is.EqualTo(msg.Id));
            Assert.That(message.Type, Is.EqualTo(msg.Type));
            messagesHandled++;
            await Task.CompletedTask;
        });

        Task.Delay(200).Wait(); // Allow some time to ensure no messages are processed

        Assert.That(messagesHandled, Is.EqualTo(0));
    }
}