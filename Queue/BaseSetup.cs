using Microsoft.Extensions.DependencyInjection;

namespace Queue;

public static class BaseSetup
{
    /// <summary>
    /// Configures the queues by registering them in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="registerQueue">The action to register a queue.</param>
    public static void ConfigureQueues(IServiceCollection services, Action<IServiceCollection, Type> registerQueue)
    {
        var messageTypes = DiscoverMessageTypes();
        foreach (var messageType in messageTypes)
        {
            registerQueue(services, messageType);
        }
    }

    /// <summary>
    /// Configures the consumers by registering them in the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="registerConsumer">The action to register a consumer.</param>
    public static void ConfigureConsumers(IServiceCollection services, Action<IServiceCollection, Type, Type, Type> registerConsumer)
    {
        var handlerTypes = DiscoverHandlerTypes();
        foreach (var handlerType in handlerTypes)
        {
            var messageType = handlerType.GetInterfaces().First().GetGenericArguments()[0];
            registerConsumer(services, messageType, handlerType, typeof(IMessageHandler<>).MakeGenericType(messageType));
        }
    }

    /// <summary>
    /// Configures a specific consumer by registering it in the service collection.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="registerConsumer">The action to register a consumer.</param>
    public static void ConfigureConsumer<T>(IServiceCollection services, Action<IServiceCollection, Type, Type, Type> registerConsumer) where T : IMessage
    {
        var handlerType = DiscoverHandlerType<T>();
        registerConsumer(services, typeof(T), handlerType, typeof(IMessageHandler<T>));
    }

    /// <summary>
    /// Discovers the message types by scanning the assemblies.
    /// </summary>
    /// <returns>A list of discovered message types.</returns>
    private static List<Type> DiscoverMessageTypes()
    {
        var messageInterfaceType = typeof(IMessage);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } && messageInterfaceType.IsAssignableFrom(t))
            .ToList();
    }

    /// <summary>
    /// Discovers the handler types by scanning the assemblies.
    /// </summary>
    /// <returns>A list of discovered handler types.</returns>
    private static List<Type> DiscoverHandlerTypes()
    {
        var handlerInterfaceType = typeof(IMessageHandler<>);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType))
            .ToList();
    }

    /// <summary>
    /// Discovers the handler type for a specific message type by scanning the assemblies.
    /// </summary>
    /// <typeparam name="T">The type of the message.</typeparam>
    /// <returns>The discovered handler type.</returns>
    private static Type DiscoverHandlerType<T>() where T : IMessage
    {
        var handlerInterfaceType = typeof(IMessageHandler<T>);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t is { IsAbstract: false, IsInterface: false } && handlerInterfaceType.IsAssignableFrom(t)) ?? throw new InvalidOperationException();
    }
}