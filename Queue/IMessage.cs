namespace Queue;

public interface IMessage
{
    public EventTypes EventType { get; set; }
}