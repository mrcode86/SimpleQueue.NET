using Queue.Models;

namespace Queue;

public interface IMessageQueue<T> where T : IMessage
{
    void Send(T message, EventTypes eventType);

    void Receive(Action<T> handleMessage);

    void CloseConnection();
}