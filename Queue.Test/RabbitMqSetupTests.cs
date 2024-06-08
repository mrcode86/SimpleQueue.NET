using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Queue.Models;
using Moq;

namespace Queue.Test
{
    [TestFixture]
    public class RabbitMqSetupTests : TestBase
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
            Assert.NotNull(handler);
            var queue = serviceProvider.GetService<IMessageQueue<MediaMessage>>();
            Assert.NotNull(queue);
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
            Assert.IsNotEmpty(hostedServices);
            Assert.IsTrue(hostedServices.Any(service => service.GetType().Name.Contains("MessageQueueHostedService")));
        }
    }
}