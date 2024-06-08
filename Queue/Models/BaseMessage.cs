namespace Queue.Models;

public enum EventTypes
{
    Added,
    Updated,
    Deleted
}

public abstract class BaseMessage : IMessage
{
    public EventTypes EventType { get; set; }
}