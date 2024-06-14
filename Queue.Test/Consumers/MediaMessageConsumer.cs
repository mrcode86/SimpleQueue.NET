using Queue.Test.Models;

namespace Queue.Test.Consumers;

public class MediaMessageConsumer : MessageConsumerBase<MediaMessage>
{
    public override async Task HandleAddedAsync(MediaMessage message)
    {
        // Handle Added Media Message
        await Task.CompletedTask; // Replace with actual async handling logic
    }

    public override async Task HandleUpdatedAsync(MediaMessage message)
    {
        // Handle Updated Media Message
        await Task.CompletedTask; // Replace with actual async handling logic
    }

    public override async Task HandleDeletedAsync(MediaMessage message)
    {
        // Handle Deleted Media Message
        await Task.CompletedTask; // Replace with actual async handling logic
    }
}