using Queue.Models;

namespace Queue;

public abstract class MessageConsumerBase<T> : IMessageHandler<T> where T : IMessage
{
    public abstract Task HandleAddedAsync(T message);

    public abstract Task HandleUpdatedAsync(T message);

    public abstract Task HandleDeletedAsync(T message);
}