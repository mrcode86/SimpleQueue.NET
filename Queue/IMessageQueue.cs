using Queue.Models;

namespace Queue;

public interface IMessageQueue<T> where T : IMessage
{
    void Send(T message, EventTypes eventType);

    void Receive(Func<T, Task> handleMessage);

    void CloseConnection();
}