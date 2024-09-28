namespace SimpleQueue;

/// <summary>
/// Handles messages of type T, routing them to appropriate handlers based on their event type.
/// </summary>
/// <typeparam name="T">The type of message to handle, must implement IMessage.</typeparam>
public class MessageHandler<T> where T : IMessage
{
    private readonly IMessageQueue<T> _messageQueue;
    private readonly IReadOnlyDictionary<EventTypes, Func<T, Task>> _eventHandlers;

    /// <summary>
    /// Initializes a new instance of the MessageHandler class.
    /// </summary>
    /// <param name="messageQueue">The queue to receive messages from.</param>
    /// <param name="messageHandler">The handler containing logic for different event types.</param>
    /// <exception cref="ArgumentNullException">Thrown if messageQueue or messageHandler is null.</exception>
    public MessageHandler(IMessageQueue<T> messageQueue, IMessageHandler<T> messageHandler)
    {
        _messageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));

        if (messageHandler == null)
            throw new ArgumentNullException(nameof(messageHandler));

        _eventHandlers = new Dictionary<EventTypes, Func<T, Task>>
        {
            [EventTypes.Added] = messageHandler.HandleAddedAsync,
            [EventTypes.Updated] = messageHandler.HandleUpdatedAsync,
            [EventTypes.Deleted] = messageHandler.HandleDeletedAsync
        };
    }

    /// <summary>
    /// Starts listening for messages in the queue and handles them asynchronously.
    /// </summary>
    public void StartListening() => _messageQueue.Receive(HandleMessageAsync);

    /// <summary>
    /// Stops listening for messages in the queue.
    /// </summary>
    public void StopListening() => _messageQueue.CloseConnection();

    private Task HandleMessageAsync(T message) =>
        _eventHandlers.TryGetValue(message.EventType, out var handler)
            ? handler(message)
            : Task.FromException(new ArgumentOutOfRangeException(nameof(message.EventType),
                $"Unsupported event type: {message.EventType}"));
}