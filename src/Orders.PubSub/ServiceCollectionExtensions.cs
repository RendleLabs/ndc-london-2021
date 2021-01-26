using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Orders.PubSub
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderPubSub(this IServiceCollection services)
        {
            var orderMessages = new OrderMessages();
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(Constants.ConnectionString));
            services.AddSingleton(orderMessages);
            services.AddSingleton<IOrderMessages>(orderMessages);
            services.AddSingleton<IOrderPublisher, OrderPublisher>();
            services.AddHostedService<OrderSubscriber>();
            return services;
        }
    }
}