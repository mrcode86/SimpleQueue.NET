using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Queue.Kafka;

/// <summary>
/// Represents a Kafka message queue implementation.
/// </summary>
/// <typeparam name="T">The type of message.</typeparam>
public class KafkaMessageQueue<T> : IMessageQueue<T> where T : IMessage
{
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;
    private readonly IConsumer<Null, string> _consumer;
    private readonly ILogger<KafkaMessageQueue<T>> _logger;

    /// <summary>
    /// Constructor to initialize KafkaMessageQueue.
    /// </summary>
    /// <param name="bootstrapServers">The Kafka bootstrap servers.</param>
    /// <param name="topic">The name of the Kafka topic.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="producer"></param>
    /// <param name="consumer"></param>
    public KafkaMessageQueue(string bootstrapServers, string topic, ILogger<KafkaMessageQueue<T>> logger,
        IProducer<Null, string> producer = null, IConsumer<Null, string> consumer = null)
    {
        if (string.IsNullOrEmpty(bootstrapServers))
            throw new ApplicationException("Bootstrap servers are missing!");

        if (string.IsNullOrEmpty(topic))
            throw new ApplicationException("Topic name is missing!");

        _logger = logger;
        _topic = topic;
        logger.LogDebug($"Creating Kafka connection for topic: {topic}");

        _producer = producer ?? new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = bootstrapServers }).Build();
        _consumer = consumer ?? new ConsumerBuilder<Null, string>(new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = $"consumer-group-{topic}",
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();
        _consumer.Subscribe(_topic);
    }

    /// <summary>
    /// Sends a message to the queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public void Send(T message, EventTypes eventType)
    {
        // Serialize message and send it to the queue
        var json = JsonSerializer.Serialize(message);
        _producer.Produce(_topic, new Message<Null, string> { Value = json });
    }

    /// <summary>
    /// Sends a message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public async Task SendAsync(T message, EventTypes eventType)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
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
                var consumeResult = _consumer.Consume();
                var json = consumeResult.Message.Value;
                var message = JsonSerializer.Deserialize<T>(json);

                if (message == null)
                {
                    _logger.LogDebug($"Received message is null. Data: {json}");
                    continue;
                }

                await handleMessage(message);
                _consumer.Commit(consumeResult);
            }
        });
    }

    /// <summary>
    /// Deletes the topic (Not applicable for Kafka).
    /// </summary>
    public void DeleteQueue()
    {
        _logger.LogWarning("Kafka does not support deleting a topic via client.");
        CloseConnection();
    }

    /// <summary>
    /// Closes the connection to the message queue.
    /// </summary>
    public void CloseConnection()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        _consumer?.Close();
        _consumer?.Dispose();
    }
}