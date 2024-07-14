using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleQueue.InMemory
{
    public static class Setup
    {
        public static void ConfigureInMemory(this IServiceCollection services, bool activateConsumers = true)
        {
            var messageHandlerTypes = new List<Type>();

            var messageHandlerInterfaceType = typeof(IMessageHandler<>);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    var interfaces = type.GetInterfaces();
                    foreach (var iface in interfaces)
                    {
                        if (iface.IsGenericType && iface.GetGenericTypeDefinition() == messageHandlerInterfaceType)
                        {
                            var messageType = iface.GetGenericArguments()[0];

                            if (!messageHandlerTypes.Contains(messageType))
                            {
                                messageHandlerTypes.Add(messageType);
                            }

                            if (activateConsumers)
                            {
                                services.AddSingleton(iface, type);
                            }
                        }
                    }
                }
            }

            foreach (var messageType in messageHandlerTypes)
            {
                var queueType = typeof(InMemoryMessageQueue<>).MakeGenericType(messageType);
                var handlerType = typeof(MessageHandler<>).MakeGenericType(messageType);
                var hostedServiceType = typeof(MessageQueueHostedService<>).MakeGenericType(messageType);

                services.AddSingleton(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
                    ActivatorUtilities.CreateInstance(provider, queueType,
                        provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(queueType))));

                if (activateConsumers)
                {
                    services.AddSingleton(handlerType, provider =>
                        ActivatorUtilities.CreateInstance(provider, handlerType,
                            provider.GetRequiredService(typeof(IMessageQueue<>).MakeGenericType(messageType)),
                            provider.GetRequiredService(typeof(IMessageHandler<>).MakeGenericType(messageType))));

                    services.AddSingleton(typeof(IHostedService), provider =>
                        ActivatorUtilities.CreateInstance(provider, hostedServiceType,
                            provider.GetRequiredService(handlerType),
                            provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(hostedServiceType))));
                }
            }
        }
    }
}