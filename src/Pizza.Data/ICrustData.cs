using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pizza.Data
{
    public interface ICrustData
    {
        Task<List<CrustEntity>> GetAsync(CancellationToken token = default);
        Task AddAsync(string id, string name, int size, decimal price, int stockCount);
        Task DecrementStockAsync(string id, CancellationToken token = default);
    }
}