using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Queue.InMemory
{
    public static class Setup
    {
        public static void ConfigureInMemory(this IServiceCollection services)
        {
            BaseSetup.ConfigureQueues(services, RegisterInMemoryQueue);
        }

        public static void ConfigureInMemoryConsumers(this IServiceCollection services)
        {
            BaseSetup.ConfigureConsumers(services, RegisterInMemoryConsumer);
        }

        public static void ConfigureInMemoryConsumer<T>(this IServiceCollection services) where T : IMessage
        {
            BaseSetup.ConfigureConsumer<T>(services, RegisterInMemoryConsumer);
        }

        private static void RegisterInMemoryQueue(IServiceCollection services, Type messageType)
        {
            var queueType = typeof(InMemoryMessageQueue<>).MakeGenericType(messageType);

            services.AddSingleton(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
                ActivatorUtilities.CreateInstance(provider, queueType,
                    provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(queueType))));
        }

        private static void RegisterInMemoryConsumer(IServiceCollection services, Type messageType, Type handlerType, Type handlerInterfaceType)
        {
            var hostedServiceType = typeof(MessageQueueHostedService<>).MakeGenericType(messageType);

            services.AddSingleton(handlerInterfaceType, handlerType);

            services.AddSingleton(typeof(IHostedService), provider =>
                ActivatorUtilities.CreateInstance(provider, hostedServiceType,
                    provider.GetRequiredService(handlerType),
                    provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(hostedServiceType))));
        }
    }
}