using Queue.Models;

namespace Queue;

public class MessageHandler<T>(IMessageQueue<T> messageQueue, IMessageHandler<T> messageHandler) where T : IMessage
{
    public void StartListening()
    {
        messageQueue.Receive(message =>
        {
            switch (message.EventType)
            {
                case EventTypes.Added:
                    messageHandler.HandleAdded(message);
                    break;

                case EventTypes.Updated:
                    messageHandler.HandleUpdated(message);
                    break;

                case EventTypes.Deleted:
                    messageHandler.HandleDeleted(message);
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