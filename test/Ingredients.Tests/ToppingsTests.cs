using System;
using System.Threading.Tasks;
using Xunit;

namespace Ingredients.Tests
{
    public class ToppingsTests : IClassFixture<IngredientsApplicationFactory>
    {
        private readonly IngredientsApplicationFactory _factory;

        public ToppingsTests(IngredientsApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetsToppings()
        {
            var client = _factory.CreateIngredientsClient();
            var response = await client.GetToppingsAsync(new GetToppingsRequest());
            Assert.Collection(response.Toppings,
                t => Assert.Equal("cheese", t.Topping.Id),
                t => Assert.Equal("sauce", t.Topping.Id)
                );
        }
    }
}
