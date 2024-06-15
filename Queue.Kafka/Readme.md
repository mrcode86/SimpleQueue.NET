# Queue.Kafka

`Queue.Kafka` is a Kafka message queue implementation designed for .NET applications. It provides a robust and scalable mechanism for sending, receiving, and processing messages using Apache Kafka as the message broker.

## Features

- **Kafka integration**: Uses Kafka for reliable message queuing.
- **Asynchronous message handling**: Supports asynchronous sending and receiving of messages.
- **Automatic consumer activation**: Automatically registers and activates message consumers.
- **Logging**: Integrated with Microsoft.Extensions.Logging for easy logging.

## Usage

### Configuring Kafka

Add the `ConfigureKafka` method in your `Startup` or `Program` class:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureKafka("your-kafka-bootstrap-servers");
        // Other service configurations
    }
}
```


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
Please make sure to update tests as appropriate.

## License
This project is licensed under the MIT License. See the [MIT](https://github.com/mrcode86/QueueExample.net/tree/main?tab=MIT-1-ov-file) file for details.