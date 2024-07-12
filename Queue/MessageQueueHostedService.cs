using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleQueue;

/// <summary>
/// Represents a hosted service for handling messages in a message queue.
/// </summary>
/// <typeparam name="T">The type of message to handle.</typeparam>
public class MessageQueueHostedService<T> : IHostedService
    where T : IMessage
{
    private readonly MessageHandler<T> _messageHandler;
    private readonly ILogger<MessageQueueHostedService<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageQueueHostedService{T}"/> class.
    /// </summary>
    /// <param name="messageHandler">The message handler to process the messages.</param>
    /// <param name="logger">The logger for logging messages.</param>
    public MessageQueueHostedService(MessageHandler<T> messageHandler, ILogger<MessageQueueHostedService<T>> logger)
    {
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Starts the message handler for processing messages.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting message handler for {MessageType}", typeof(T).Name);
        _messageHandler.StartListening();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the message handler from processing messages.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping message handler for {MessageType}", typeof(T).Name);
        _messageHandler.StopListening();
        return Task.CompletedTask;
    }
}