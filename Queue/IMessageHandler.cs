using Queue.Models;

namespace Queue;

public interface IMessageHandler<in T> where T : IMessage
{
    void HandleAdded(T message);

    void HandleUpdated(T message);

    void HandleDeleted(T message);
}