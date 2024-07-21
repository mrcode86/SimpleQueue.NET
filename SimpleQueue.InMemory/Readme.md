# SimpleQueue.InMemory

`SimpleQueue.InMemory` is a simple in-memory message queue implementation designed for .NET applications. It provides a lightweight and easy-to-use mechanism for sending, receiving, and processing messages within the application without the need for external dependencies like RabbitMQ or Kafka.

## Features

- **In-memory storage**: Keeps messages in memory using a concurrent queue.
- **Asynchronous message handling**: Supports asynchronous sending and receiving of messages.
- **Logging**: Integrated with Microsoft.Extensions.Logging for easy logging.
- **Message consumers**: Handles messages based on their event types (Added, Updated, Deleted).

## Usage

### Setup

To configure the in-memory message queue, use the `RegisterQueueHandlersAndServices` method in your `Startup` class or wherever you configure your services.

```csharp
using Microsoft.Extensions.DependencyInjection;
using SimpleQueue.InMemory;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.RegisterQueueHandlersAndServices(assemblies);
    }
}
```

### Define Messages
Create a message class that inherits from `BaseMessage`:

```csharp
using SimpleQueue;

public class MyMessage : BaseMessage
{
    public string Content { get; set; }
}
```

### Implement Message Handlers
Implement the `IMessageHandler<T>` interface to handle your messages:

```csharp
using SimpleQueue;
using System.Threading.Tasks;

public class MyMessageHandler : IMessageHandler<MyMessage>
{
    public Task HandleAddedAsync(MyMessage message)
    {
        // Handle added message
        return Task.CompletedTask;
    }

    public Task HandleUpdatedAsync(MyMessage message)
    {
        // Handle updated message
        return Task.CompletedTask;
    }

    public Task HandleDeletedAsync(MyMessage message)
    {
        // Handle deleted message
        return Task.CompletedTask;
    }
}
```

### Send Messages
Inject the `IMessageQueue<MyMessage>` into your service or controller and use it to send messages:

```csharp
using SimpleQueue;
using System.Threading.Tasks;

public class MyService
{
    private readonly IMessageQueue<MyMessage> _messageQueue;

    public MyService(IMessageQueue<MyMessage> messageQueue)
    {
        _messageQueue = messageQueue;
    }

    public Task SendMessageAsync()
    {
        var message = new MyMessage { Content = "Hello, World!", EventType = EventTypes.Added };
        return _messageQueue.SendAsync(message, message.EventType);
    }
}
```

### Receive Messages
The message queue will automatically start receiving messages and pass them to the appropriate handlers based on their event types.

### Logging
`SimpleQueue.InMemory` uses `Microsoft.Extensions.Logging` for logging. Make sure to configure logging in your application to see the log messages.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.RegisterQueueHandlersAndServices();
                services.AddLogging(configure => configure.AddConsole());
            })
            .Build();

        host.Run();
    }
}
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
Please make sure to update tests as appropriate.

## License
This project is licensed under the MIT License. See the [MIT](https://choosealicense.com/licenses/mit/) file for details.

