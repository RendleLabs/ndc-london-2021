using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Pizza.Data
{
    public class CrustData : ICrustData
    {
        private readonly ILogger<CrustData> _log;
        private const string TableName = "crusts";
        private readonly CloudTable _table;

        public CrustData(ILogger<CrustData> log)
        {
            _log = log;
            var credentials = new StorageCredentials(Constants.AccountName, Constants.AccountKey);
            var client = new CloudTableClient(new Uri(Constants.BaseUri), credentials);
            _table = client.GetTableReference(TableName);
        }

        public async Task<List<CrustEntity>> GetAsync(CancellationToken token = default)
        {
            try
            {
                var query = _table.CreateQuery<CrustEntity>();
                var list = await query.ExecuteAsync(token).ToListAsync(token);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error reading data.");
                throw;
            }
        }

        public async Task AddAsync(string id, string name, int size, decimal price, int stockCount)
        {
            try
            {
                var entity = new CrustEntity(id, name, size, price, stockCount);
                var insert = TableOperation.Insert(entity);
                await _table.ExecuteAsync(insert);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error inserting data.");
                throw;
            }
        }

        public async Task DecrementStockAsync(string id, CancellationToken token = default)
        {
            for (int i = 0; i < 100; i++)
            {
                var retrieve = TableOperation.Retrieve<CrustEntity>("crust", id);
                var result = await _table.ExecuteAsync(retrieve, token);
                if (!result.IsSuccessStatusCode() || result.Result is null)
                {
                    _log.LogWarning("Entity not found: {Id}", id);
                    return;
                }

                var entity = (CrustEntity) result.Result;
                if (entity.StockCount == 0) return;
                entity.StockCount -= 1;
                var update = TableOperation.Replace(entity);
                try
                {
                    await _table.ExecuteAsync(update, token);
                    break;
                }
                catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == 412)
                {
                    _log.LogInformation("Conflict updating entity, retrying.");
                }
            }
        }
    }
}