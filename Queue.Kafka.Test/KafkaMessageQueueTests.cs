using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace Queue.Kafka.Test;

public class KafkaMessageQueueTests
{
    private KafkaMessageQueue<TestMessage> _kafkaMessageQueue;
    private Mock<IProducer<Null, string>> _mockProducer;
    private Mock<IConsumer<Null, string>> _mockConsumer;
    private Mock<ILogger<KafkaMessageQueue<TestMessage>>> _mockLogger;

    [SetUp]
    public void SetUp()
    {
        var conf = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var queueConnectionString = conf.GetConnectionString("MyConnectionString");

        _mockProducer = new Mock<IProducer<Null, string>>();
        _mockConsumer = new Mock<IConsumer<Null, string>>();
        _mockLogger = new Mock<ILogger<KafkaMessageQueue<TestMessage>>>();

        _kafkaMessageQueue = new KafkaMessageQueue<TestMessage>(
            queueConnectionString,
            "test-topic",
            _mockLogger.Object,
            _mockProducer.Object,
            _mockConsumer.Object
        );
    }

    [Test]
    public void Send_ShouldProduceMessage()
    {
        var message = new TestMessage { EventType = EventTypes.Added, Text = "Hello, Kafka!" };
        _kafkaMessageQueue.Send(message, EventTypes.Added);

        _mockProducer.Verify(p => p.Produce(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<Action<DeliveryReport<Null, string>>>()), Times.Once);
    }

    [Test]
    public async Task SendAsync_ShouldProduceMessageAsync()
    {
        var message = new TestMessage { EventType = EventTypes.Added, Text = "Hello, Kafka!" };
        await _kafkaMessageQueue.SendAsync(message, EventTypes.Added);

        _mockProducer.Verify(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), CancellationToken.None), Times.Once);
    }

    [Test]
    public void Receive_ShouldConsumeMessage()
    {
        var message = new TestMessage { EventType = EventTypes.Added, Text = "Hello, Kafka!" };
        var json = JsonSerializer.Serialize(message);

        _mockConsumer.Setup(c => c.Consume(It.IsAny<CancellationToken>()))
            .Returns(new ConsumeResult<Null, string> { Message = new Message<Null, string> { Value = json } });

        _kafkaMessageQueue.Receive(async msg =>
        {
            Assert.AreEqual(message.Text, msg.Text);
            await Task.CompletedTask;
        });

        _mockConsumer.Verify(c => c.Consume(It.IsAny<CancellationToken>()), Times.Once);
    }
}