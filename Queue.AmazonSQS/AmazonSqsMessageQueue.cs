using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;

namespace SimpleQueue.AmazonSQS;

/// <summary>
/// Represents an Amazon SQS message queue implementation.
/// </summary>
/// <typeparam name="T">The type of message.</typeparam>
public class AmazonSqsMessageQueue<T> : IMessageQueue<T> where T : IMessage
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _queueUrl;
    private readonly ILogger<AmazonSqsMessageQueue<T>> _logger;

    /// <summary>
    /// Constructor to initialize AmazonSqsMessageQueue.
    /// </summary>
    /// <param name="queueUrl">The URL of the SQS queue.</param>
    /// <param name="sqsClient">The Amazon SQS client.</param>
    /// <param name="logger">The logger instance.</param>
    public AmazonSqsMessageQueue(string queueUrl, IAmazonSQS sqsClient, ILogger<AmazonSqsMessageQueue<T>> logger)
    {
        _queueUrl = queueUrl;
        _sqsClient = sqsClient;
        _logger = logger;
        _logger.LogDebug($"Creating Amazon SQS queue for URL: {queueUrl}");
    }

    /// <summary>
    /// Sends a message to the queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public void Send(T message, EventTypes eventType)
    {
        var json = JsonSerializer.Serialize(message);
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = json
        };
        _sqsClient.SendMessageAsync(sendMessageRequest).Wait();
    }

    /// <summary>
    /// Sends a message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public async Task SendAsync(T message, EventTypes eventType)
    {
        var json = JsonSerializer.Serialize(message);
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = json
        };
        await _sqsClient.SendMessageAsync(sendMessageRequest);
    }

    /// <summary>
    /// Receives a message from the queue and handles it asynchronously.
    /// </summary>
    /// <param name="handleMessage">The handler function to process the received message.</param>
    public void Receive(Func<T, Task> handleMessage)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20
                };
                var response = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);
                foreach (var sqsMessage in response.Messages)
                {
                    var message = JsonSerializer.Deserialize<T>(sqsMessage.Body);
                    if (message != null)
                    {
                        await handleMessage(message);
                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            ReceiptHandle = sqsMessage.ReceiptHandle
                        };
                        await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
                    }
                }
                await Task.Delay(100);
            }
        });
    }

    /// <summary>
    /// Deletes the queue from Amazon SQS.
    /// </summary>
    public void DeleteQueue()
    {
        _sqsClient.DeleteQueueAsync(new DeleteQueueRequest { QueueUrl = _queueUrl }).Wait();
        _logger.LogInformation($"Queue {_queueUrl} deleted.");
    }

    /// <summary>
    /// Closes the connection to the message queue.
    /// </summary>
    public void CloseConnection()
    {
        // Amazon SQS client doesn't need explicit closure
        _logger.LogInformation("Amazon SQS connection closed (no-op).");
    }
}