using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Queue.Models;

namespace Queue;

public class MessageQueueHostedService<T>(
    MessageHandler<T> messageHandler,
    ILogger<MessageQueueHostedService<T>> logger)
    : IHostedService
    where T : IMessage
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting message handler for {MessageType}", typeof(T).Name);
        messageHandler.StartListening();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping message handler for {MessageType}", typeof(T).Name);
        messageHandler.StopListening();
        return Task.CompletedTask;
    }
}