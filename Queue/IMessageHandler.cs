using Queue.Models;

namespace Queue;

public interface IMessageHandler<in T> where T : IMessage
{
    Task HandleAddedAsync(T message);

    Task HandleUpdatedAsync(T message);

    Task HandleDeletedAsync(T message);
}