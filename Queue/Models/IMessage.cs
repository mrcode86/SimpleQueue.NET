namespace Queue.Models;

public interface IMessage
{
    public EventTypes EventType { get; set; }
}