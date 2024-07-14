using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SimpleQueue.AmazonSQS.Test.Helpers;

namespace SimpleQueue.AmazonSQS.Test;

public class AmazonSqsMessageQueueTests
{
    private AmazonSqsMessageQueue<TestMessage> _amazonSqsMessageQueue;
    private Mock<IAmazonSQS> _mockSqsClient;
    private InMemoryLogger<AmazonSqsMessageQueue<TestMessage>> _inMemoryLogger;

    [SetUp]
    public void SetUp()
    {
        _mockSqsClient = new Mock<IAmazonSQS>();
        _inMemoryLogger = new InMemoryLogger<AmazonSqsMessageQueue<TestMessage>>();

        _amazonSqsMessageQueue = new AmazonSqsMessageQueue<TestMessage>(
            "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue",
            _mockSqsClient.Object,
            _inMemoryLogger
        );
    }

    [Test]
    public async Task SendAsync_ShouldSendMessageAsync()
    {
        var message = new TestMessage { EventType = EventTypes.Added, Text = "Hello, SQS!" };
        await _amazonSqsMessageQueue.SendAsync(message, EventTypes.Added);

        _mockSqsClient.Verify(s => s.SendMessageAsync(It.Is<SendMessageRequest>(req =>
            req.MessageBody.Contains("Hello, SQS!")),
            default),
            Times.Once);
    }

    [Test]
    public void Receive_ShouldReceiveAndHandleMessage()
    {
        var message = new TestMessage { EventType = EventTypes.Added, Text = "Hello, SQS!" };
        var json = JsonSerializer.Serialize(message);

        _mockSqsClient.SetupSequence(s => s.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), default))
                      .ReturnsAsync(new ReceiveMessageResponse
                      {
                          Messages = [new() { Body = json, ReceiptHandle = "test-receipt-handle" }]
                      })
                      .ReturnsAsync(new ReceiveMessageResponse { Messages = new List<Message>() }); // Return empty to stop the loop

        var handled = false;
        _amazonSqsMessageQueue.Receive(async msg =>
        {
            Assert.That(msg.Text, Is.EqualTo(message.Text));
            handled = true;
            await Task.CompletedTask;
        });

        // Simulate wait for message handling
        Task.Delay(100).Wait();

        Assert.That(handled, Is.True);
        _mockSqsClient.Verify(s => s.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), default), Times.Once);
    }

    [Test]
    public void DeleteQueue_ShouldDeleteQueue()
    {
        _amazonSqsMessageQueue.DeleteQueue();

        _mockSqsClient.Verify(s => s.DeleteQueueAsync(It.Is<DeleteQueueRequest>(req =>
            req.QueueUrl == "https://sqs.us-east-1.amazonaws.com/123456789012/test-queue"),
            default),
            Times.Once);

        Assert.That(_inMemoryLogger.LogEntries.Exists(log =>
            log.LogLevel == LogLevel.Information &&
            log.Message.Contains("Queue https://sqs.us-east-1.amazonaws.com/123456789012/test-queue deleted.")), Is.True);
    }

    [Test]
    public void CloseConnection_ShouldLogClosure()
    {
        _amazonSqsMessageQueue.CloseConnection();

        Assert.That(_inMemoryLogger.LogEntries.Exists(log =>
            log.LogLevel == LogLevel.Information &&
            log.Message.Contains("Amazon SQS connection closed (no-op).")), Is.True);
    }
}