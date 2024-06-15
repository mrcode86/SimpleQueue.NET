using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Queue.Kafka.Test;

public class SetupTest : TestBase
{
    [Test]
    public void ConfigureKafka_ShouldRegisterServices()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        services.ConfigureKafka(QueueConnectionString);

        var provider = services.BuildServiceProvider();

        var messageQueue = provider.GetService<IMessageQueue<TestMessage>>();
        Assert.That(messageQueue, Is.Not.Null);
        Assert.That(messageQueue, Is.InstanceOf<KafkaMessageQueue<TestMessage>>());

        var handler = provider.GetService<IMessageHandler<TestMessage>>();
        Assert.That(handler, Is.Not.Null);

        var hostedService = provider.GetService<IHostedService>();
        Assert.That(hostedService, Is.Not.Null);
    }

    [Test]
    public void ConfigureKafka_ShouldActivateConsumers()
    {
        var services = new ServiceCollection();
        var loggerMock = new Mock<ILogger<MessageQueueHostedService<TestMessage>>>();

        services.ConfigureKafka(QueueConnectionString);

        var provider = services.BuildServiceProvider();
        var hostedService = provider.GetService<IHostedService>();

        Assert.That(hostedService, Is.Not.Null);
        Assert.That(hostedService, Is.InstanceOf<MessageQueueHostedService<TestMessage>>());
    }
}