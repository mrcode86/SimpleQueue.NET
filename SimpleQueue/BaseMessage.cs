namespace SimpleQueue;

public enum EventTypes
{
    Added,
    Updated,
    Deleted
}

/// <summary>
/// Represents a base message for the queue.
/// </summary>
public abstract class BaseMessage : IMessage
{
    /// <summary>
    /// Gets or sets the event type of the message.
    /// </summary>
    public EventTypes EventType { get; set; }
}