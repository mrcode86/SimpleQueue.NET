﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Queue.Models;

namespace Queue.InMemory;

public static class Setup
{
    /// <summary>
    /// Configures in-memory message handling by registering message queues.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies.</param>
    public static void ConfigureInMemory(this IServiceCollection services)
    {
        // Create a list to keep track of all different types of messages
        var messageTypes = new List<Type>();

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
                        if (!messageTypes.Contains(messageType))
                        {
                            messageTypes.Add(messageType);
                        }
                    }
                }
            }
        }

        // Register message queues for each message type
        foreach (var messageType in messageTypes)
        {
            RegisterMessageQueue(services, messageType);
        }
    }

    /// <summary>
    /// Registers in-memory message queue for a specific message type.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies.</param>
    /// <param name="messageType">The type of message.</param>
    private static void RegisterMessageQueue(IServiceCollection services, Type messageType)
    {
        var queueType = typeof(InMemoryMessageQueue<>).MakeGenericType(messageType);

        // Register the message queue
        services.AddSingleton(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
            ActivatorUtilities.CreateInstance(provider, queueType,
                provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(queueType))));
    }

    /// <summary>
    /// Configures in-memory consumers by registering message handlers and background services for all discovered message types.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies.</param>
    public static void ConfigureInMemoryConsumers(this IServiceCollection services)
    {
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
                        RegisterMessageHandlerAndConsumer(services, messageType, type, iface);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Registers in-memory message handler and consumer for a specific message type.
    /// </summary>
    /// <typeparam name="T">The type of message.</typeparam>
    /// <param name="services">The service collection to register the dependencies.</param>
    public static void ConfigureInMemoryConsumer<T>(this IServiceCollection services) where T : IMessage
    {
        var messageHandlerInterfaceType = typeof(IMessageHandler<T>);

        // Use reflection to find the implementation of IMessageHandler<T>
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var handlerType = assemblies.SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => messageHandlerInterfaceType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        if (handlerType != null)
        {
            RegisterMessageHandlerAndConsumer(services, typeof(T), handlerType, messageHandlerInterfaceType);
        }
    }

    /// <summary>
    /// Registers in-memory message handler and consumer for a specific message type.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies.</param>
    /// <param name="messageType">The type of message.</param>
    /// <param name="handlerType">The type of the message handler.</param>
    /// <param name="handlerInterfaceType">The interface type of the message handler.</param>
    private static void RegisterMessageHandlerAndConsumer(IServiceCollection services, Type messageType, Type handlerType, Type handlerInterfaceType)
    {
        var hostedServiceType = typeof(MessageQueueHostedService<>).MakeGenericType(messageType);

        // Register the message handler
        services.AddSingleton(handlerInterfaceType, handlerType);

        // Register the background service that listens for messages
        services.AddSingleton(typeof(IHostedService), provider =>
            ActivatorUtilities.CreateInstance(provider, hostedServiceType,
                provider.GetRequiredService(handlerType),
                provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(hostedServiceType))));
    }
}
