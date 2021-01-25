using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly IngredientsService.IngredientsServiceClient _client;
        private readonly ILogger<HomeController> _log;

        public HomeController(IngredientsService.IngredientsServiceClient client, ILogger<HomeController> log)
        {
            _client = client;
            _log = log;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var toppingsResponse = await _client.GetToppingsAsync(new GetToppingsRequest());
            var toppings = toppingsResponse.Toppings
                .Select(t => new ToppingViewModel(t.Topping.Id, t.Topping.Name, (decimal) t.Topping.Price))
                .ToList();

            var crustsResponse = await _client.GetCrustsAsync(new GetCrustsRequest());
            var crusts = crustsResponse.Crusts
                .Select(c => new CrustViewModel(c.Crust.Id, c.Crust.Name, c.Crust.Size, (decimal) c.Crust.Price))
                .ToList();
            
            var viewModel = new HomeViewModel(toppings, crusts);
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
