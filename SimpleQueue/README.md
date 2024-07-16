# SimpleQueue.NET

## Overview
SimpleQueue is a lightweight library for managing message queues in .NET applications. It provides a straightforward and flexible way to send, receive, and process messages asynchronously. The library supports handling different event types and integrates seamlessly with dependency injection and logging in ASP.NET Core applications.

## Features
- Define and handle various types of events (Added, Updated, Deleted).
- Abstract base classes and interfaces for defining messages and message handlers.
- Easy integration with .NET's IHostedService for background processing.
- Built-in support for logging and dependency injection in ASP.NET Core applications.
- Supports both synchronous and asynchronous message sending and handling.
- Provides a consistent and simple API for queue management.

## Installation

Install the SimpleQueue package via NuGet:

```bash
dotnet add package SimpleQueue.NET
```

## Usage

### Define a Message
Create a class that represents the message you want to send. The message class should inherit from the `BaseMessage` base class and define any properties that are required for the message.

```csharp
using SimpleQueue;

public class MyMessage : BaseMessage
{
	public string Content { get; set; }
}
```

### Define a Message Handler
Create a class that implements the `IMessageHandler<T>` interface, where `T` is the message type. Implement the `HandleAddedAsync`, `HandleUpdatedAsync`, `HandleDeletedAsync` methods to define how the message should be processed.

```csharp
using SimpleQueue;
using System.Threading.Tasks;

public class MyMessageHandler : MessageConsumerBase<MyMessage>
{
    public override Task HandleAddedAsync(MyMessage message)
    {
        // Handle the added message
        Console.WriteLine($"Added: {message.Content}");
        return Task.CompletedTask;
    }

    public override Task HandleUpdatedAsync(MyMessage message)
    {
        // Handle the updated message
        Console.WriteLine($"Updated: {message.Content}");
        return Task.CompletedTask;
    }

    public override Task HandleDeletedAsync(MyMessage message)
    {
        // Handle the deleted message
        Console.WriteLine($"Deleted: {message.Content}");
        return Task.CompletedTask;
    }
}
```

### Configure Message Queue

Implement the `IMessageQueue` interface for your specific message queue backend.


```csharp
using SimpleQueue;
using System;
using System.Threading.Tasks;

public class InMemoryMessageQueue<T> : IMessageQueue<T> where T : IMessage
{
    private readonly ConcurrentQueue<T> _queue = new();

    public void Send(T message, EventTypes eventType)
    {
        _queue.Enqueue(message);
    }

    public Task SendAsync(T message, EventTypes eventType)
    {
        Send(message, eventType);
        return Task.CompletedTask;
    }

    public void Receive(Func<T, Task> handleMessage)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                if (_queue.TryDequeue(out var message))
                {
                    await handleMessage(message);
                }
                await Task.Delay(100); // Adding a small delay to prevent busy-waiting
            }
        });
    }

    public void DeleteQueue()
    {
        _queue.Clear();
    }

    public void CloseConnection()
    {
    }
}
```

### Register Services

Register the message queue and message handler in the DI container.

```csharp
using Microsoft.Extensions.DependencyInjection;
using SimpleQueue;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
            services.AddSingleton<IMessageQueue<MyMessage>, InMemoryMessageQueue<MyMessage>>();
            services.AddSingleton<IMessageHandler<MyMessage>, MyMessageHandler>();
            services.AddSingleton<MessageHandler<MyMessage>>();
            services.AddHostedService<MessageQueueHostedService<MyMessage>>();
	}
}
```

### Run the Application
Run your ASP.NET Core application, and the message queue will start processing messages in the background.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
Please make sure to update tests as appropriate.

## License
This project is licensed under the MIT License. See the [MIT](https://choosealicense.com/licenses/mit/) file for details.
