namespace SimpleQueue;

/// <summary>
/// Represents a message interface.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Gets or sets the event type of the message.
    /// </summary>
    EventTypes EventType { get; set; }
}