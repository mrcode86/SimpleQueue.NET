using Queue.Models;

namespace Queue;

public class MessageHandler<T>(IMessageQueue<T> messageQueue, IMessageHandler<T> messageHandler) where T : IMessage
{
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

    public void StopListening()
    {
        messageQueue.CloseConnection();
    }

    public void SendMessage(T message, EventTypes eventType)
    {
        messageQueue.Send(message, eventType);
    }
}