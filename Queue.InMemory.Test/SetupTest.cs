using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace SimpleQueue.InMemory.Test;

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
        Assert.That(messageQueue.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));

        // Check if MessageHandler<TestMessage> is registered
        var messageHandler = services.FirstOrDefault(d => d.ServiceType == typeof(MessageHandler<TestMessage>));
        Assert.That(messageHandler, Is.Not.Null);
        Assert.That(messageHandler.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
        
        // Check if MessageQueueHostedService<TestMessage> is registered
        var hostedService = services.FirstOrDefault(d => d.ServiceType == typeof(IHostedService) && d.ImplementationType == typeof(MessageQueueHostedService<TestMessage>));
        Assert.That(hostedService, Is.Not.Null);
        Assert.That(hostedService.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
    }
}

public class TestMessage : BaseMessage;