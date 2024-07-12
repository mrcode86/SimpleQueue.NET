using Queue.Demo.Web.Models;
using SimpleQueue;

namespace Queue.Demo.Web.Consumers;

public class TestMessageHandler : IMessageHandler<TestMessage>
{
    public static List<TestMessage> Messages { get; } = new();
    public static event Action? OnMessagesChanged;

    public async Task HandleAddedAsync(TestMessage message)
    {
        Messages.Add(message);
        OnMessagesChanged?.Invoke();

        Console.WriteLine($"Received message: {message.Text}");
        await Task.CompletedTask;
    }

    public async Task HandleUpdatedAsync(TestMessage message)
    {
        Messages.Add(message);
        OnMessagesChanged?.Invoke();

        Console.WriteLine($"Received message: {message.Text}");
        await Task.CompletedTask;
    }

    public async Task HandleDeletedAsync(TestMessage message)
    {
        Messages.Add(message);
        OnMessagesChanged?.Invoke();

        Console.WriteLine($"Received message: {message.Text}");
        await Task.CompletedTask;
    }
}