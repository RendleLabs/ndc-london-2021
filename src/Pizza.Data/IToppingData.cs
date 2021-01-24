using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza.Data
{
    public interface IToppingData
    {
        Task<List<ToppingEntity>> GetAsync(CancellationToken token = default);
        Task DecrementStockAsync(string id, CancellationToken token = default);
    }
}