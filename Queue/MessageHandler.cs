namespace Queue;

/// <summary>
/// This class represents a base message consumer for handling messages of type T.
/// It is an abstract class that implements the IMessageHandler interface.
/// The class provides abstract methods for handling added, updated, and deleted messages asynchronously.
/// Subclasses of this class will implement these methods to define the specific logic for handling the messages.
/// </summary>
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

                default:
                    throw new ArgumentOutOfRangeException();
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
}