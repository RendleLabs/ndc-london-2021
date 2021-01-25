using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Pizza.Data;
using TestHelpers;

namespace Ingredients.Tests
{
    public class IngredientsApplicationFactory : WebApplicationFactory<Startup>
    {
        public IngredientsService.IngredientsServiceClient CreateIngredientsClient()
        {
            var channel = this.CreateGrpcChannel();
            return new IngredientsService.IngredientsServiceClient(channel);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IToppingData>();

                var list = new List<ToppingEntity>
                {
                    new("cheese", "Cheese", 0.5m, 50),
                    new("sauce", "Sauce", 0.5m, 50),
                };

                var sub = Substitute.For<IToppingData>();
                sub.GetAsync(Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(list));
                
                services.AddSingleton(sub);
            });
        }
    }
}