using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orders.PubSub;

namespace Orders.Services
{
    public class OrdersImpl : OrdersService.OrdersServiceBase
    {
        private static readonly ActivitySource ActSource = new("Orders");
        
        private readonly IngredientsService.IngredientsServiceClient _ingredients;
        private readonly IOrderPublisher _orderPublisher;
        private readonly IOrderMessages _orderMessages;
        private readonly ILogger<OrdersImpl> _logger;

        public OrdersImpl(IngredientsService.IngredientsServiceClient ingredients, IOrderPublisher orderPublisher, IOrderMessages orderMessages, ILogger<OrdersImpl> logger)
        {
            _ingredients = ingredients;
            _orderPublisher = orderPublisher;
            _orderMessages = orderMessages;
            _logger = logger;
        }

        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
        {
            var now = DateTimeOffset.UtcNow;

            using var activity = ActSource.StartActivity("PlaceOrder");

            activity?.SetTag("crust", request.CrustId);
            activity?.SetTag("toppings", string.Join(", ", request.ToppingIds));

            await _orderPublisher.PublishOrder(request.CrustId, request.ToppingIds, now);
            
            await Task.WhenAll(
                DecrementCrustsAsync(request),
                DecrementToppings(request));

            return new PlaceOrderResponse
            {
                Time = now.ToTimestamp()
            };
        }

        public override async Task Subscribe(SubscribeRequest request, IServerStreamWriter<SubscribeResponse> responseStream, ServerCallContext context)
        {
            var cancellationToken = context.CancellationToken;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var orderMessage = await _orderMessages.ReadAsync(cancellationToken);
                    var response = new SubscribeResponse
                    {
                        CrustId = orderMessage.CrustId,
                        ToppingIds = { orderMessage.ToppingIds },
                        Time = orderMessage.Time.ToTimestamp()
                    };
                    await responseStream.WriteAsync(response);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Subscriber disconnected.");
                    break;
                }
            }
        }

        private async Task DecrementToppings(PlaceOrderRequest request)
        {
            var decrementToppingsRequest = new DecrementToppingsRequest
            {
                ToppingIds = {request.ToppingIds}
            };
            await _ingredients.DecrementToppingsAsync(decrementToppingsRequest);
        }

        private async Task DecrementCrustsAsync(PlaceOrderRequest request)
        {
            var decrementCrustRequest = new DecrementCrustsRequest
            {
                CrustId = request.CrustId
            };
            await _ingredients.DecrementCrustsAsync(decrementCrustRequest);
        }
    }
}