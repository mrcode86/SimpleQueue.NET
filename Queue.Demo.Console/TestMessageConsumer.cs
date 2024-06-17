namespace Queue.Demo.Console;

public class TestMessageConsumer : MessageConsumerBase<TestMessage>
{
    public override async Task HandleAddedAsync(TestMessage message)
    {
        System.Console.WriteLine($"Received message: {message.Text}");
        await Task.CompletedTask; // Replace with actual async handling logic
    }

    public override async Task HandleUpdatedAsync(TestMessage message)
    {
        // Handle Updated Test Message
        await Task.CompletedTask; // Replace with actual async handling logic
    }

    public override async Task HandleDeletedAsync(TestMessage message)
    {
        // Handle Deleted Test Message
        await Task.CompletedTask; // Replace with actual async handling logic
    }
}