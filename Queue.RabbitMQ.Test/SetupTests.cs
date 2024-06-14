using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Queue.Test;
using Queue.Test.Models;

namespace Queue.RabbitMQ.Test;

[TestFixture]
public class SetupTests : TestBase
{
    [Test]
    public void ConfigureServices_RegistersMessageHandlers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Mock ILogger
        var loggerMock = new Mock<ILogger<RabbitMqMessageQueue<MediaMessage>>>();
        services.AddSingleton(loggerMock.Object);

        // Act
        services.ConfigureRabbitMq(QueueConnectionString);

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var handler = serviceProvider.GetService<IMessageHandler<MediaMessage>>();
        Assert.That(handler, Is.Not.Null);

        var queue = serviceProvider.GetService<IMessageQueue<MediaMessage>>();
        Assert.That(queue, Is.Not.Null);
    }

    [Test]
    public void ConfigureServices_RegistersHostedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Mock ILogger for each type dynamically
        services.AddLogging();

        // Act
        services.ConfigureRabbitMq(QueueConnectionString);

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var hostedServices = serviceProvider.GetServices<IHostedService>();
        Assert.That(hostedServices, Is.Not.Empty);
        Assert.That(hostedServices.Any(service => service.GetType().Name.Contains("MessageQueueHostedService")), Is.True);
    }
}