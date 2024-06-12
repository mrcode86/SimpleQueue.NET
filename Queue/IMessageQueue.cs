using Queue.Models;

namespace Queue;

public interface IMessageQueue<T> where T : IMessage
{
    /// <summary>
    /// Sends a message to the queue with the specified event type.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    void Send(T message, EventTypes eventType);

    /// <summary>
    /// Sends a message to the queue with the specified event type asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="eventType">The event type of the message.</param>
    Task SendAsync(T message, EventTypes eventType);

    /// <summary>
    /// Receives a message from the queue and handles it asynchronously.
    /// </summary>
    /// <param name="handleMessage">The handler function to process the received message.</param>
    void Receive(Func<T, Task> handleMessage);

    /// <summary>
    /// Deletes the queue.
    /// </summary>
    void DeleteQueue();

    /// <summary>
    /// Closes the connection to the message queue.
    /// </summary>
    void CloseConnection();
}