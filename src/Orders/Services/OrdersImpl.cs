using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Orders.PubSub;

namespace Orders.Services
{
    public class OrdersImpl : OrdersService.OrdersServiceBase
    {
        private readonly IngredientsService.IngredientsServiceClient _ingredients;
        private readonly IOrderPublisher _orderPublisher;

        public OrdersImpl(IngredientsService.IngredientsServiceClient ingredients, IOrderPublisher orderPublisher)
        {
            _ingredients = ingredients;
            _orderPublisher = orderPublisher;
        }

        public override async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request, ServerCallContext context)
        {
            var now = DateTimeOffset.UtcNow;

            await _orderPublisher.PublishOrder(request.CrustId, request.ToppingIds, now);
            
            await Task.WhenAll(
                DecrementCrustsAsync(request),
                DecrementToppings(request));

            return new PlaceOrderResponse
            {
                Time = now.ToTimestamp()
            };
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