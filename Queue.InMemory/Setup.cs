using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Queue.InMemory
{
    public static class Setup
    {
        public static void ConfigureInMemory(this IServiceCollection services)
        {
            var messageHandlerInterfaceType = typeof(IMessageHandler<>);

            // Hämta alla assemblys för att söka efter handler-typer
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes()
                                          .Where(t => !t.IsAbstract && !t.IsInterface)
                                          .Where(t => t.GetInterfaces()
                                                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == messageHandlerInterfaceType));

                foreach (var handlerType in handlerTypes)
                {
                    var messageType = handlerType.GetInterfaces()
                                                 .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == messageHandlerInterfaceType)
                                                 .GetGenericArguments()[0];

                    RegisterInMemoryQueue(services, messageType);
                    RegisterInMemoryConsumer(services, messageType, handlerType);
                }
            }
        }

        private static void RegisterInMemoryQueue(IServiceCollection services, Type messageType)
        {
            var queueType = typeof(InMemoryMessageQueue<>).MakeGenericType(messageType);
            var loggerType = typeof(ILogger<>).MakeGenericType(queueType);
            services.AddSingleton(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
                ActivatorUtilities.CreateInstance(provider, queueType,
                    provider.GetRequiredService(loggerType)));
        }

        private static void RegisterInMemoryConsumer(IServiceCollection services, Type messageType, Type handlerType)
        {
            var consumerInterfaceType = typeof(IMessageHandler<>).MakeGenericType(messageType);
            var hostedServiceType = typeof(MessageQueueHostedService<>).MakeGenericType(messageType);

            services.AddSingleton(consumerInterfaceType, handlerType);
            services.AddSingleton(typeof(IHostedService), provider =>
            {
                var loggerType = typeof(ILogger<>).MakeGenericType(hostedServiceType);
                var handlerInstance = ActivatorUtilities.CreateInstance(provider, handlerType); // Create instance of handler

                return ActivatorUtilities.CreateInstance(provider, hostedServiceType,
                    handlerInstance,
                    provider.GetRequiredService(loggerType));
            });
        }
    }
}
