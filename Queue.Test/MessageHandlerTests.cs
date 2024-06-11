using Moq;
using NUnit.Framework;
using Queue.Models;

namespace Queue.Test;

[TestFixture]
public class MessageHandlerTests
{
    private Mock<IMessageQueue<MediaMessage>> _mockMessageQueue;
    private Mock<IMessageHandler<MediaMessage>> _mockMessageHandler;
    private MessageHandler<MediaMessage> _messageHandler;

    [SetUp]
    public void Setup()
    {
        _mockMessageQueue = new Mock<IMessageQueue<MediaMessage>>();
        _mockMessageHandler = new Mock<IMessageHandler<MediaMessage>>();
        _messageHandler = new MessageHandler<MediaMessage>(_mockMessageQueue.Object, _mockMessageHandler.Object);
    }

    [Test]
    public void Add_ShouldSetEventTypeToAddedAndSendMessage()
    {
        // Arrange
        var mediaMessage = new MediaMessage
        {
            Id = "1",
            Type = "Video"
        };

        // Act
        _messageHandler.Add(mediaMessage);

        // Assert
        Assert.That(mediaMessage.EventType, Is.EqualTo(EventTypes.Added));
        _mockMessageQueue.Verify(mq => mq.Send(mediaMessage, EventTypes.Added), Times.Once);
    }

    [Test]
    public void Update_ShouldSetEventTypeToUpdatedAndSendMessage()
    {
        // Arrange
        var mediaMessage = new MediaMessage
        {
            Id = "1",
            Type = "Video"
        };

        // Act
        _messageHandler.Update(mediaMessage);

        // Assert
        Assert.That(mediaMessage.EventType, Is.EqualTo(EventTypes.Updated));
        _mockMessageQueue.Verify(mq => mq.Send(mediaMessage, EventTypes.Updated), Times.Once);
    }

    [Test]
    public void Delete_ShouldSetEventTypeToDeletedAndSendMessage()
    {
        // Arrange
        var mediaMessage = new MediaMessage
        {
            Id = "1",
            Type = "Video"
        };

        // Act
        _messageHandler.Delete(mediaMessage);

        // Assert
        Assert.That(mediaMessage.EventType, Is.EqualTo(EventTypes.Deleted));
        _mockMessageQueue.Verify(mq => mq.Send(mediaMessage, EventTypes.Deleted), Times.Once);
    }

    [Test]
    public async Task AddAsync_ShouldSetEventTypeToAddedAndSendMessage()
    {
        // Arrange
        var mediaMessage = new MediaMessage
        {
            Id = "1",
            Type = "Video"
        };

        // Act
        await _messageHandler.AddAsync(mediaMessage);

        // Assert
        Assert.That(mediaMessage.EventType, Is.EqualTo(EventTypes.Added));
        _mockMessageQueue.Verify(mq => mq.SendAsync(mediaMessage, EventTypes.Added), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ShouldSetEventTypeToUpdatedAndSendMessage()
    {
        // Arrange
        var mediaMessage = new MediaMessage
        {
            Id = "1",
            Type = "Video"
        };

        // Act
        await _messageHandler.UpdateAsync(mediaMessage);

        // Assert
        Assert.That(mediaMessage.EventType, Is.EqualTo(EventTypes.Updated));
        _mockMessageQueue.Verify(mq => mq.SendAsync(mediaMessage, EventTypes.Updated), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ShouldSetEventTypeToDeletedAndSendMessage()
    {
        // Arrange
        var mediaMessage = new MediaMessage
        {
            Id = "1",
            Type = "Video"
        };

        // Act
        await _messageHandler.DeleteAsync(mediaMessage);

        // Assert
        Assert.That(mediaMessage.EventType, Is.EqualTo(EventTypes.Deleted));
        _mockMessageQueue.Verify(mq => mq.SendAsync(mediaMessage, EventTypes.Deleted), Times.Once);
    }
}