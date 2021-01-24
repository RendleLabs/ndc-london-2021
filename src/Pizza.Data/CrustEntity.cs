using Microsoft.Azure.Cosmos.Table;

namespace Pizza.Data
{
    public class CrustEntity : TableEntity
    {
        public CrustEntity()
        {
            PartitionKey = "crust";
        }
        
        public CrustEntity(string id, string name, int size, decimal price, int stockCount) : this()
        {
            Id = id;
            Name = name;
            Size = size;
            Price = price;
            StockCount = stockCount;
        }

        [IgnoreProperty]
        public string Id
        {
            get => RowKey;
            set => RowKey = value;
        }
        public string Name { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
    }
}