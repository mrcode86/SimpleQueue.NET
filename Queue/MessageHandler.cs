using Queue.Models;

namespace Queue;

public class MessageHandler<T>(IMessageQueue<T> messageQueue, IMessageHandler<T> messageHandler) where T : IMessage
{
    /// <summary>
    /// Starts listening for messages in the queue and handles them asynchronously.
    /// </summary>
    public void StartListening()
    {
        messageQueue.Receive(async message =>
        {
            switch (message.EventType)
            {
                case EventTypes.Added:
                    await messageHandler.HandleAddedAsync(message);
                    break;

                case EventTypes.Updated:
                    await messageHandler.HandleUpdatedAsync(message);
                    break;

                case EventTypes.Deleted:
                    await messageHandler.HandleDeletedAsync(message);
                    break;
            }
        });
    }

    /// <summary>
    /// Stops listening for messages in the queue.
    /// </summary>
    public void StopListening()
    {
        messageQueue.CloseConnection();
    }

    /// <summary>
    /// Sends an added message to the queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Add(T message)
    {
        message.EventType = EventTypes.Added;
        messageQueue.Send(message, message.EventType);
    }

    /// <summary>
    /// Sends an updated message to the queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Update(T message)
    {
        message.EventType = EventTypes.Updated;
        messageQueue.Send(message, message.EventType);
    }

    /// <summary>
    /// Sends a deleted message to the queue.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public void Delete(T message)
    {
        message.EventType = EventTypes.Deleted;
        messageQueue.Send(message, message.EventType);
    }

    /// <summary>
    /// Sends an added message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public async Task AddAsync(T message)
    {
        message.EventType = EventTypes.Added;
        await messageQueue.SendAsync(message, message.EventType);
    }

    /// <summary>
    /// Sends an updated message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public async Task UpdateAsync(T message)
    {
        message.EventType = EventTypes.Updated;
        await messageQueue.SendAsync(message, message.EventType);
    }

    /// <summary>
    /// Sends a deleted message to the queue asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    public async Task DeleteAsync(T message)
    {
        message.EventType = EventTypes.Deleted;
        await messageQueue.SendAsync(message, message.EventType);
    }
}