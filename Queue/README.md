# RabbitMQ Message Queue

This project provides a simple implementation of a message queue using RabbitMQ in C#. It includes classes for sending and receiving messages to and from a RabbitMQ server.

## Features

- Sending messages to a RabbitMQ queue
- Receiving messages from a RabbitMQ queue
- Handling different types of events (Added, Updated, Deleted)
- Logging message handling events

## Usage

### Initializing the RabbitMQ Message Queue

First, you need to initialize the `RabbitMqMessageQueue` class with the connection string and queue name:

```csharp
var connectionString = "your_rabbitmq_connection_string";
var queueName = "your_queue_name";

var messageQueue = new RabbitMqMessageQueue<YourMessageType>(connectionString, queueName, logger);
```

### Sending Messages
To send a message to the RabbitMQ queue, you can use the Send method:

```csharp
var message = new YourMessageType { /* Initialize your message properties */ };
messageQueue.Send(message, EventTypes.Added);
```

### Receiving Messages
To receive messages from the RabbitMQ queue, you can use the Receive method:

```csharp
messageQueue.Receive((message, eventType) =>
{
	// Handle the received message
	Console.WriteLine($"Received message: {message}");
});
```

### Closing Connection
It's important to close the RabbitMQ connection when it's no longer needed. You can use the CloseConnection method for this:

```csharp
messageQueue.CloseConnection();
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
Please make sure to update tests as appropriate.

## License
This project is licensed under the MIT License. See the [MIT](https://choosealicense.com/licenses/mit/) file for details.
