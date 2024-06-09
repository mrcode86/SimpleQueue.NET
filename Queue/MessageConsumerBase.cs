using Queue.Models;

namespace Queue;

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