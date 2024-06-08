using Queue.Models;

namespace Queue;

public abstract class MessageConsumerBase<T> : IMessageHandler<T> where T : IMessage
{
    public abstract void HandleAdded(T message);

    public abstract void HandleUpdated(T message);

    public abstract void HandleDeleted(T message);
}