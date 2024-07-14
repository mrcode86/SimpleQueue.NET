namespace SimpleQueue;

/// <summary>
/// This class represents a base message consumer for handling messages of type T.
/// It is an abstract class that implements the IMessageHandler interface.
/// The class provides abstract methods for handling added, updated, and deleted messages asynchronously.
/// Subclasses of this class will implement these methods to define the specific logic for handling the messages.
/// </summary>
public abstract class MessageConsumerBase<T> : IMessageHandler<T> where T : IMessage
{
    /// <summary>
    /// Handles the added message asynchronously.
    /// </summary>
    /// <param name="message">The added message to handle.</param>
    public abstract Task HandleAddedAsync(T message);

    /// <summary>
    /// Handles the updated message asynchronously.
    /// </summary>
    /// <param name="message">The updated message to handle.</param>
    public abstract Task HandleUpdatedAsync(T message);

    /// <summary>
    /// Handles the deleted message asynchronously.
    /// </summary>
    /// <param name="message">The deleted message to handle.</param>
    public abstract Task HandleDeletedAsync(T message);
}