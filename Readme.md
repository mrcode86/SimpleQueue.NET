# SimpleQueue.NET

## Overview

This project provides an example implementation of a message queue system using different backends like RabbitMQ and InMemory queues. The system is designed to be extensible, allowing easy integration of new message queue implementations. The project also includes a setup for configuring and consuming messages from the queues.

## Project Structure

- `SimpleQueue`: Contains the main interfaces and base setup classes.
- `SimpleQueue.RabbitMQ`: Contains the RabbitMQ-specific implementations and setup classes.
- `SimpleQueue.InMemory`: Contains the InMemory-specific implementations and setup classes.
- `SimpleQueue.Kafka`: Contains the Kafka-specific implementations and setup classes.
- `SimpleQueue.Redis`: Contains the Redis-specific implementations and setup classes (not implemented).
- `SimpleQueue.AmazonSQS`: Contains the Amazon SQS-specific implementations and setup classes (not done).
- `SimpleQueue.Demo.Console`: Contains a demo console application to demonstrate the usage of the message queue system.
- `SimpleQueue.Demo.Web`: Contains a demo web application to demonstrate the usage of the message queue system.
- `SimpleQueue.Demo.ServiceDefaults`: Contains the default service configurations for the demo applications. (.NET Aspire)
- `SimpleQueue.Demo.AppHost`: Contains the host configuration for the demo applications. (.NET Aspire)
- `SimpleQueue.Test`: Contains helper classes and methods for testing the project.
- `SimpleQueue.*.Test`: Contains unit tests for the project.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (if using RabbitMQ backend)
- [Redis](https://redis.io/download) (if using Redis backend, though it isn't implemented in this example)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/mrcode86/SimpleQueue.NET.git
   cd SimpleQueue.NET
2. Restore the dependencies:
   ```bash
   dotnet restore
   ```
1. Build the solution:
   ```bash
   dotnet build
   ```

### Configuration
Configure the services in your Program.cs or Startup.cs file as follows:

```csharp
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure RabbitMQ
        services.ConfigureRabbitMq("your-rabbitmq-connection-string");

        // Configure InMemory queue
        services.ConfigureInMemory();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    });

await builder.RunConsoleAsync();
```

### Usage

#### Sending Messages
To send a message to the queue, you need to inject the IMessageQueue<T> interface and use the Send method:

```csharp
public class MessageProducer
{
    private readonly IMessageQueue<YourMessageModel> _messageQueue;

    public MessageProducer(IMessageQueue<YourMessageModel> messageQueue)
    {
        _messageQueue = messageQueue;
    }

    public void ProduceMessage(YourMessageModel message)
    {
        _messageQueue.Send(message, EventTypes.SomeEventType);
    }
}
```

#### Consuming Messages
To consume messages, implement the IMessageHandler<T> interface and handle the messages accordingly:

```csharp
public class YourMessageHandler : IMessageHandler<YourMessageModel>
{
    public Task HandleAsync(YourMessageModel message)
    {
        // Handle the message
        return Task.CompletedTask;
    }
}
```

#### Running Tests
To run the tests, use the following command:

```bash
dotnet test
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
Please make sure to update tests as appropriate.

## License
This project is licensed under the MIT License. See the [MIT](https://choosealicense.com/licenses/mit/) file for details.
