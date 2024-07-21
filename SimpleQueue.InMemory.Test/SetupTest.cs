using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace SimpleQueue.InMemory.Test
{
    [TestFixture]
    public class SetupTests
    {
        private IServiceCollection _services;
        private Mock<ILogger<InMemoryMessageQueue<TestMessage>>> _mockLoggerQueue;
        private Mock<ILogger<MessageQueueHostedService<TestMessage>>> _mockLoggerHostedService;

        [SetUp]
        public void SetUp()
        {
            _services = new ServiceCollection();
            _mockLoggerQueue = new Mock<ILogger<InMemoryMessageQueue<TestMessage>>>();
            _mockLoggerHostedService = new Mock<ILogger<MessageQueueHostedService<TestMessage>>>();
        }

        [Test]
        public void RegisterQueueHandlersAndServices_ShouldRegisterMessageHandlers()
        {
            // Arrange
            _services.AddSingleton(_mockLoggerQueue.Object);
            _services.AddSingleton(_mockLoggerHostedService.Object);

            // Act
            _services.RegisterQueueHandlersAndServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var handler = serviceProvider.GetService<IMessageHandler<TestMessage>>();
            Assert.That(handler, Is.Not.Null);
            Assert.That(handler, Is.InstanceOf<IMessageHandler<TestMessage>>());
        }

        [Test]
        public void RegisterQueueHandlersAndServices_ShouldRegisterHostedService()
        {
            // Arrange
            _services.AddSingleton(_mockLoggerQueue.Object);
            _services.AddSingleton(_mockLoggerHostedService.Object);

            // Act
            _services.RegisterQueueHandlersAndServices();
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var hostedService = serviceProvider.GetService<IHostedService>();
            Assert.That(hostedService, Is.Not.Null);
            Assert.That(hostedService, Is.InstanceOf<MessageQueueHostedService<TestMessage>>());
        }

        public class TestMessage : IMessage
        {
            public EventTypes EventType { get; set; }
        }
    }
}