using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace Queue.InMemory.Test;

public class SetupTests
{
    [Test]
    public void ConfigureInMemory_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        services.ConfigureInMemory();

        // Assert
        // Check if IMessageQueue<TestMessage> is registered
        var messageQueue = services.FirstOrDefault(d => d.ServiceType == typeof(IMessageQueue<TestMessage>));
        Assert.That(messageQueue, Is.Not.Null);
        Assert.That(ServiceLifetime.Singleton, Is.EqualTo(messageQueue.Lifetime));

        // Check if MessageHandler<TestMessage> is registered
        var messageHandler = services.FirstOrDefault(d => d.ServiceType == typeof(MessageHandler<TestMessage>));
        Assert.That(messageHandler, Is.Not.Null);
        Assert.That(ServiceLifetime.Singleton, Is.EqualTo(messageHandler.Lifetime));
        
        // Check if MessageQueueHostedService<TestMessage> is registered
        var hostedService = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(MessageQueueHostedService<TestMessage>));
        Assert.That(hostedService, Is.Not.Null);
        Assert.That(ServiceLifetime.Singleton, Is.EqualTo(hostedService.Lifetime));
    }
}

public class TestMessage : BaseMessage;