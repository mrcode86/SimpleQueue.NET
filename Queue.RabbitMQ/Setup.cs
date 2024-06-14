using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Queue.RabbitMQ
{
    public static class Setup
    {
        public static void ConfigureRabbitMq(this IServiceCollection services, string connectionString)
        {
            BaseSetup.ConfigureQueues(services, (s, messageType) => RegisterRabbitMqQueue(s, connectionString, messageType));
        }

        public static void ConfigureRabbitMqConsumers(this IServiceCollection services)
        {
            BaseSetup.ConfigureConsumers(services, RegisterRabbitMqConsumer);
        }

        public static void ConfigureRabbitMqConsumer<T>(this IServiceCollection services) where T : IMessage
        {
            BaseSetup.ConfigureConsumer<T>(services, RegisterRabbitMqConsumer);
        }

        private static void RegisterRabbitMqQueue(IServiceCollection services, string connectionString, Type messageType)
        {
            var queueType = typeof(RabbitMqMessageQueue<>).MakeGenericType(messageType);

            services.AddSingleton(typeof(IMessageQueue<>).MakeGenericType(messageType), provider =>
                ActivatorUtilities.CreateInstance(provider, queueType, connectionString, messageType.Name,
                    provider.GetRequiredService(typeof(ILogger<>).MakeGenericType(queueType))));
        }

        private static void RegisterRabbitMqConsumer(IServiceCollection services, Type messageType, Type handlerType, Type handlerInterfaceType)
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