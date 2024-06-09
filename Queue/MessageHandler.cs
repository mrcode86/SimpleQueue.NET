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
    /// Sends a message to the queue with the specified event type.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    public void SendMessage(T message, EventTypes eventType)
    {
        messageQueue.Send(message, eventType);
    }
}