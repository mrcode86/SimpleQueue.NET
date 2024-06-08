using Queue.Models;

namespace Queue.Consumers;

public class MediaMessageConsumer : MessageConsumerBase<MediaMessage>
{
    public override void HandleAdded(MediaMessage message)
    {
        // Handle Added Media Message
    }

    public override void HandleUpdated(MediaMessage message)
    {
        // Handle Updated Media Message
    }

    public override void HandleDeleted(MediaMessage message)
    {
        // Handle Deleted Media Message
    }
}