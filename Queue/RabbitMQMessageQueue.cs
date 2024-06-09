﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Queue.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Queue;

public class RabbitMqMessageQueue<T> : IMessageQueue<T> where T : IMessage
{
    private IModel? _channel;
    private readonly string _queueName;
    private IConnection? _connection;
    private readonly ILogger<RabbitMqMessageQueue<T>> _logger;

    public RabbitMqMessageQueue(string connectionString, string queueName, ILogger<RabbitMqMessageQueue<T>> logger)
    {
        _logger = logger;
        logger.LogDebug("Creating RabbitMQ connection for queue: " + queueName);

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString),
            DispatchConsumersAsync = true // Use async dispatcher
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = queueName;

        _channel.QueueDeclare(queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void Send(T message, EventTypes eventType)
    {
        // Serialize message and send it to the queue
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        _channel.BasicPublish(exchange: "",
            routingKey: _queueName,
            basicProperties: null,
            body: body);
    }

    public void Receive(Func<T, Task> handleMessage)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));

            if (message == null)
            {
                _logger.LogDebug("Received message is null. Data:" + Encoding.UTF8.GetString(body));
                return;
            }

            await handleMessage(message);
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(queue: _queueName,
            autoAck: false, // Enable manual acks
            consumer: consumer);
    }

    public void CloseConnection()
    {
        _channel?.Close();
        _channel = null;
        _connection?.Close();
        _connection = null;
    }
}