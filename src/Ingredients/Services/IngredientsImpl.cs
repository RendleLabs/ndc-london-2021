using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Pizza.Data;

namespace Ingredients.Services
{
    internal class IngredientsImpl : IngredientsService.IngredientsServiceBase
    {
        private readonly IToppingData _toppingData;
        private readonly ICrustData _crustData;

        public IngredientsImpl(IToppingData toppingData, ICrustData crustData)
        {
            _toppingData = toppingData;
            _crustData = crustData;
        }

        public override async Task<GetToppingsResponse> GetToppings(GetToppingsRequest request, ServerCallContext context)
        {
            var toppings = await _toppingData.GetAsync(context.CancellationToken);
            var availableToppings = toppings.Select(t =>
                new AvailableTopping
                {
                    Quantity = t.StockCount,
                    Topping = new Topping
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Price = t.Price
                    }
                });

            var response = new GetToppingsResponse
            {
                Toppings = {availableToppings}
            };
            
            return response;
        }

        public override async Task<GetCrustsResponse> GetCrusts(GetCrustsRequest request, ServerCallContext context)
        {
            var crusts = await _crustData.GetAsync(context.CancellationToken);
            var availableCrusts = crusts.Select(t =>
                new AvailableCrust
                {
                    Quantity = t.StockCount,
                    Crust = new Crust
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Size = t.Size,
                        Price = t.Price
                    }
                });

            var response = new GetCrustsResponse
            {
                Crusts = { availableCrusts }
            };

            return response;
        }

        public override async Task<DecrementToppingsResponse> DecrementToppings(DecrementToppingsRequest request, ServerCallContext context)
        {
            var tasks = request.ToppingIds.Select(id => _toppingData.DecrementStockAsync(id));

            await Task.WhenAll(tasks);

            return new DecrementToppingsResponse();
        }

        public override async Task<DecrementCrustsResponse> DecrementCrusts(DecrementCrustsRequest request, ServerCallContext context)
        {
            await _crustData.DecrementStockAsync(request.CrustId);
            return new DecrementCrustsResponse();
        }
    }
}