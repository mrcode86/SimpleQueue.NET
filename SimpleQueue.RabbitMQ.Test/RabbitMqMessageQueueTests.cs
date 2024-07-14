using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleQueue.Test.Models;
using System.Text;
using System.Text.Json;

namespace SimpleQueue.RabbitMQ.Test;

[TestFixture]
public class RabbitMqMessageQueueTests
{
    private Mock<IConnection> _mockConnection;
    private Mock<IModel> _mockChannel;
    private Mock<ILogger<RabbitMqMessageQueue<MediaMessage>>> _mockLogger;
    private RabbitMqMessageQueue<MediaMessage> _queue;
    private readonly string _queueName = "TestQueue";
    protected IConfiguration Configuration;
    protected string QueueConnectionString;

    [SetUp]
    public void Setup()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        QueueConnectionString = Configuration.GetConnectionString("MyConnectionString")!;

        _mockConnection = new Mock<IConnection>();
        _mockChannel = new Mock<IModel>();
        _mockLogger = new Mock<ILogger<RabbitMqMessageQueue<MediaMessage>>>();

        _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

        _queue = new RabbitMqMessageQueue<MediaMessage>(_mockConnection.Object, _queueName, _mockLogger.Object);
    }

    [Test]
    public void Send_MessageSent_Successfully()
    {
        // Arrange
        var message = new MediaMessage();

        // Act
        _queue.Send(message, EventTypes.Added);

        // Assert
        _mockChannel.Verify(c => c.BasicPublish(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<IBasicProperties>(),
            It.IsAny<byte[]>()
        ), Times.Once);

        _queue.DeleteQueue();
    }

    [Test]
    public async Task Receive_MessageReceived_Successfully()
    {
        // Arrange
        var originalMessage = new MediaMessage { Id = "1", Type = "test" };
        var messageHandled = false;
        var messageReceivedEvent = new TaskCompletionSource<bool>();
        MediaMessage? receivedMessage = null;

        _mockChannel.Setup(c => c.BasicConsume(
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<bool>(),
            It.IsAny<IDictionary<string, object>>(),
            It.IsAny<IBasicConsumer>()
        )).Callback<string, bool, string, bool, bool, IDictionary<string, object>, IBasicConsumer>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer) =>
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(originalMessage));
            var ea = new BasicDeliverEventArgs { Body = body };

            Task.Run(() =>
            {
                ((AsyncEventingBasicConsumer)consumer).HandleBasicDeliver(
                    consumerTag,
                    deliveryTag: 1,
                    redelivered: false,
                    exchange: "",
                    routingKey: queue,
                    properties: null,
                    body: ea.Body
                );
            });
        });

        // Act
        _queue.Receive(async msg =>
        {
            receivedMessage = msg;
            messageHandled = true;
            messageReceivedEvent.SetResult(true);
            await Task.CompletedTask;
        });

        await _queue.SendAsync(originalMessage, EventTypes.Added);

        // Wait for the message to be received or for a timeout
        if (!await messageReceivedEvent.Task.TimeoutAfter(TimeSpan.FromSeconds(5)))
        {
            Assert.Fail("Timeout waiting for message to be received.");
        }

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