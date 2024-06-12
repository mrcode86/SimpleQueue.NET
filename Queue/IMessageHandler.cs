namespace Queue;

public interface IMessageHandler<in T> where T : IMessage
{
    /// <summary>
    /// Handles the added message asynchronously.
    /// </summary>
    /// <param name="message">The added message to handle.</param>
    Task HandleAddedAsync(T message);

    /// <summary>
    /// Handles the updated message asynchronously.
    /// </summary>
    /// <param name="message">The updated message to handle.</param>
    Task HandleUpdatedAsync(T message);

    /// <summary>
    /// Handles the deleted message asynchronously.
    /// </summary>
    /// <param name="message">The deleted message to handle.</param>
    Task HandleDeletedAsync(T message);
}