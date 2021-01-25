using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orders;

namespace Frontend.Controllers
{
    [Route("orders")]
    public class OrdersController : Controller
    {
        private readonly OrdersService.OrdersServiceClient _orders;
        private readonly ILogger<OrdersController> _log;

        public OrdersController(ILogger<OrdersController> log, OrdersService.OrdersServiceClient orders)
        {
            _log = log;
            _orders = orders;
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromForm]HomeViewModel viewModel)
        {
            var request = new PlaceOrderRequest
            {
                CrustId = viewModel.SelectedCrust,
                ToppingIds =
                {
                    viewModel.Toppings
                        .Where(t => t.Selected)
                        .Select(t => t.Id)
                }
            };

            await _orders.PlaceOrderAsync(request);
            return View();
        }
    }
}