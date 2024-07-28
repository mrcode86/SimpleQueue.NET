using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SimpleQueue.RabbitMQ;

/// <summary>
/// Represents a RabbitMQ message queue implementation.
/// </summary>
/// <typeparam name="T">The type of message.</typeparam>
public class RabbitMqMessageQueue<T> : IMessageQueue<T> where T : IMessage
{
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly ILogger<RabbitMqMessageQueue<T>> _logger;

    /// <summary>
    /// Constructor to initialize RabbitMqMessageQueue.
    /// </summary>
    /// <param name="connection">The shared RabbitMQ connection.</param>
    /// <param name="queueName">The name of the queue.</param>
    /// <param name="logger">The logger instance.</param>
    public RabbitMqMessageQueue(IConnection connection, string queueName, ILogger<RabbitMqMessageQueue<T>> logger)
    {
        if (connection == null)
            throw new ApplicationException("RabbitMQ connection is missing!");

        if (string.IsNullOrEmpty(queueName))
            throw new ApplicationException("Queue name is missing!");

        _logger = logger;
        _logger.LogDebug($"Creating RabbitMQ channel for queue: {queueName}");

        _channel = connection.CreateModel();
        _queueName = queueName;

        _channel.QueueDeclare(queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    /// <summary>
    /// Sends a message to the queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public void Send(T message, EventTypes eventType)
    {
        message.EventType = eventType;

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "",
            routingKey: _queueName,
            basicProperties: null,
            body: body);
    }

    /// <summary>
    /// Sends a message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public async Task SendAsync(T message, EventTypes eventType)
    {
        message.EventType = eventType;

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        await Task.Run(() =>
        {
            _channel.BasicPublish(exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: body);
        });
    }

    /// <summary>
    /// Receives a message from the queue and handles it asynchronously.
    /// </summary>
    /// <param name="handleMessage">The handler function to process the received message.</param>
    public void Receive(Func<T, Task> handleMessage)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(json);

            if (message == null)
            {
                _logger.LogDebug($"Received message is null. Data: {Encoding.UTF8.GetString(body)}");
                return;
            }

            await handleMessage(message);
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(queue: _queueName,
            autoAck: false,
            consumer: consumer);
    }

    /// <summary>
    /// Deletes the queue from RabbitMQ.
    /// </summary>
    public void DeleteQueue()
    {
        _channel.QueueDelete(_queueName);
        _logger.LogInformation($"Queue {_queueName} deleted.");
    }

    /// <summary>
    /// Closes the connection to the message queue.
    /// </summary>
    public void CloseConnection()
    {
        _channel.Close();
    }
}