using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Reflection;

namespace SimpleQueue.RabbitMQ;

/// <summary>
/// Provides methods to configure RabbitMQ connection and register queue handlers and services.
/// </summary>
public static class Setup
{
    /// <summary>
    /// Configures RabbitMQ connection as a singleton in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The RabbitMQ connection string.</param>
    public static void ConfigureRabbitMq(this IServiceCollection services, string connectionString)
    {
        // Register the RabbitMQ connection as a singleton
        services.AddSingleton<IConnection>(provider =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString),
                DispatchConsumersAsync = true // Use async dispatcher
            };
            return factory.CreateConnection();
        });
    }

    /// <summary>
    /// Registers queue handlers and services in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for message handlers.</param>
    /// <param name="activateConsumers">Indicates whether to activate consumers.</param>
    public static void RegisterQueueHandlersAndServices(this IServiceCollection services, Assembly[] assemblies, bool activateConsumers = true)
    {
        // Create a list to keep track of all different types of messages
        var messageHandlerTypes = new List<Type>();

        // Use reflection to find all implementations of IMessageHandler<T>
        var messageHandlerInterfaceType = typeof(IMessageHandler<>);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                // Find all concrete types that implement IMessageHandler<T>
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var interfaces = type.GetInterfaces();

                foreach (var iface in interfaces)
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == messageHandlerInterfaceType)
                    {
                        var messageType = iface.GetGenericArguments()[0];

                        // Add the message type to the list
                        if (!messageHandlerTypes.Contains(messageType))
                            messageHandlerTypes.Add(messageType);

                        // Register the message handler
                        if (activateConsumers)
                            services.AddScoped(iface, type); // Change to Scoped
                    }
                }
            }
        }

        foreach (var messageType in messageHandlerTypes)
        {
            var queueType = typeof(RabbitMqMessageQueue<>).MakeGenericType(messageType);
            var handlerType = typeof(MessageHandler<>).MakeGenericType(messageType);
            var hostedServiceType = typeof(MessageQueueHostedService<>).MakeGenericType(messageType);

            services.AddScoped(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
                ActivatorUtilities.CreateInstance(provider, queueType, provider.GetRequiredService<IConnection>(), messageType.Name,
                    provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(queueType))));

            if (activateConsumers)
            {
                // Register the MessageHandler
                services.AddScoped(handlerType);

                // Register the background service that listens for messages
                services.AddSingleton(typeof(IHostedService), provider =>
                    ActivatorUtilities.CreateInstance(provider, hostedServiceType,
                        provider.CreateScope().ServiceProvider.GetRequiredService(handlerType),
                        provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(hostedServiceType))));
            }
        }
    }
}