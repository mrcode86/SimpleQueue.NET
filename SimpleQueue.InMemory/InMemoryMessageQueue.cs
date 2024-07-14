using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SimpleQueue.InMemory
{
    /// <summary>
    /// Represents an in-memory message queue implementation.
    /// </summary>
    /// <typeparam name="T">The type of message.</typeparam>
    public class InMemoryMessageQueue<T> : IMessageQueue<T> where T : IMessage
    {
        private readonly ConcurrentQueue<T> _queue = new();
        private readonly ILogger<InMemoryMessageQueue<T>> _logger;

        /// <summary>
        /// Constructor to initialize InMemoryMessageQueue.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public InMemoryMessageQueue(ILogger<InMemoryMessageQueue<T>> logger)
        {
            _logger = logger;
            _logger.LogDebug("Creating in-memory queue.");
        }

        /// <summary>
        /// Sends a message to the queue.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="eventType">The event type of the message.</param>
        public void Send(T message, EventTypes eventType)
        {
            _queue.Enqueue(message);
            _logger.LogInformation($"Message of type {eventType} enqueued.");
        }

        /// <summary>
        /// Sends a message to the queue asynchronously.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="eventType">The event type of the message.</param>
        public Task SendAsync(T message, EventTypes eventType)
        {
            Send(message, eventType);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Receives a message from the queue and handles it asynchronously.
        /// </summary>
        /// <param name="handleMessage">The handler function to process the received message.</param>
        public void Receive(Func<T, Task> handleMessage)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (_queue.TryDequeue(out var message))
                    {
                        await handleMessage(message);
                    }
                    await Task.Delay(100); // Adding a small delay to prevent busy-waiting
                }
            });
        }

        /// <summary>
        /// Deletes all messages in the queue.
        /// </summary>
        public void DeleteQueue()
        {
            _queue.Clear();
            _logger.LogInformation("In-memory queue cleared.");
        }

        /// <summary>
        /// Closes the connection to the message queue.
        /// </summary>
        public void CloseConnection()
        {
            // No-op for in-memory queue
            _logger.LogInformation("In-memory queue connection closed (no-op).");
        }
    }
}