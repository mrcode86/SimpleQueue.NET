using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using System.Reflection;

namespace SimpleQueue.RabbitMQ.Test;

[TestFixture]
public class SetupTests
{
    private IServiceCollection _services;
    private Mock<IConnection> _mockConnection;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _mockConnection = new Mock<IConnection>();
    }

    [Test]
    public void ConfigureRabbitMq_ShouldRegisterSingletonIConnection()
    {
        // Arrange
        const string connectionString = "amqp://guest:guest@localhost:5672/";

        // Act
        _services.ConfigureRabbitMq(connectionString);
        var serviceProvider = _services.BuildServiceProvider();
        var connection = serviceProvider.GetService<IConnection>();

        // Assert
        Assert.That(connection, Is.Not.Null);
        Assert.That(connection, Is.InstanceOf<IConnection>());
    }

    [Test]
    public void ConfigureRabbitMq_ShouldThrowExceptionForInvalidUri()
    {
        // Arrange
        const string invalidConnectionString = "invalid_uri";

        // Act & Assert
        Assert.Throws<UriFormatException>(() => _services.ConfigureRabbitMq(invalidConnectionString));
    }

    [Test]
    public void RegisterRabbitMqHandlersAndServices_ShouldRegisterHandlersAndServices()
    {
        // Arrange
        var assemblies = new[] { Assembly.GetExecutingAssembly() };
        _services.AddSingleton(_mockConnection.Object);

        // Act
        _services.RegisterQueueHandlersAndServices(assemblies);
        var serviceProvider = _services.BuildServiceProvider();
        var handlers = serviceProvider.GetServices<IHostedService>();

        // Assert
        var hostedServices = handlers as IHostedService[] ?? handlers.ToArray();
        Assert.That(hostedServices, Is.Not.Null);
        Assert.That(hostedServices, Is.Not.Empty);
    }

    [Test]
    public void RegisterRabbitMqHandlersAndServices_ShouldRegisterScopedHandlers()
    {
        // Arrange
        var assemblies = new[] { Assembly.GetExecutingAssembly() };
        _services.AddSingleton(_mockConnection.Object);

        // Act
        _services.RegisterQueueHandlersAndServices(assemblies, activateConsumers: true);
        var serviceProvider = _services.BuildServiceProvider();

        var handler = serviceProvider.GetService<IMessageHandler<TestMessage>>();

        // Assert
        Assert.That(handler, Is.Not.Null);
        Assert.That(handler, Is.InstanceOf<IMessageHandler<TestMessage>>());
    }

    public class TestMessage : IMessage
    {
        public EventTypes EventType { get; set; }
    }

    public class TestMessageHandler : IMessageHandler<TestMessage>
    {
        public Task HandleAddedAsync(TestMessage message) => Task.CompletedTask;

        public Task HandleUpdatedAsync(TestMessage message) => Task.CompletedTask;

        public Task HandleDeletedAsync(TestMessage message) => Task.CompletedTask;
    }
}