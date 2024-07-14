﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace SimpleQueue.RabbitMQ;

public static class Setup
{
    public static void ConfigureRabbitMq(this IServiceCollection services, string connectionString, bool activateConsumers = true)
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

        services.ConfigureRabbitMq(activateConsumers);
    }

    public static void ConfigureRabbitMq(this IServiceCollection services, bool activateConsumers = true)
    {
        // Create a list to keep track of all different types of messages
        var messageHandlerTypes = new List<Type>();

        // Use reflection to find all implementations of IMessageHandler<T>
        var messageHandlerInterfaceType = typeof(IMessageHandler<>);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

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
                        {
                            messageHandlerTypes.Add(messageType);
                        }

                        if (activateConsumers)
                        {
                            // Register the message handler
                            services.AddSingleton(iface, type);
                        }
                    }
                }
            }
        }

        // Register message queues and consumers
        foreach (var messageType in messageHandlerTypes)
        {
            var queueType = typeof(RabbitMqMessageQueue<>).MakeGenericType(messageType);
            var handlerType = typeof(MessageHandler<>).MakeGenericType(messageType);
            var hostedServiceType = typeof(MessageQueueHostedService<>).MakeGenericType(messageType);

            // Register the message queue
            services.AddSingleton(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
                ActivatorUtilities.CreateInstance(provider, queueType, provider.GetRequiredService<IConnection>(), messageType.Name,
                    provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(queueType))));

            if (activateConsumers)
            {
                // Register the MessageHandler
                services.AddSingleton(handlerType);

                // Register the background service that listens for messages
                services.AddSingleton(typeof(IHostedService), provider =>
                    ActivatorUtilities.CreateInstance(provider, hostedServiceType,
                        provider.GetRequiredService(handlerType),
                        provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(hostedServiceType))));
            }
        }
    }
}