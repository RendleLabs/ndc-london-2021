using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Pizza.Data;

namespace Ingredients.Services
{
    internal class IngredientsImpl : IngredientsService.IngredientsServiceBase
    {
        private readonly IToppingData _data;

        public IngredientsImpl(IToppingData data)
        {
            _data = data;
        }

        public override async Task<GetToppingsResponse> GetToppings(GetToppingsRequest request, ServerCallContext context)
        {
            var toppings = await _data.GetAsync(context.CancellationToken);
            var availableToppings = toppings.Select(t =>
                new AvailableTopping
                {
                    Quantity = t.StockCount,
                    Topping = new Topping
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Price = (double)t.Price
                    }
                });

            var response = new GetToppingsResponse
            {
                Toppings = {availableToppings}
            };

            return response;
        }
    }
}