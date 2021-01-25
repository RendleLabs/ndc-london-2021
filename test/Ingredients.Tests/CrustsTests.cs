using System.Threading.Tasks;
using Xunit;

namespace Ingredients.Tests
{
    public class CrustsTests : IClassFixture<IngredientsApplicationFactory>
    {
        private readonly IngredientsApplicationFactory _factory;

        public CrustsTests(IngredientsApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetsCrusts()
        {
            var client = _factory.CreateIngredientsClient();
            var response = await client.GetCrustsAsync(new GetCrustsRequest());
            Assert.Collection(response.Crusts,
                t => Assert.Equal("thin", t.Crust.Id),
                t => Assert.Equal("deep", t.Crust.Id)
            );
        }
    }
}