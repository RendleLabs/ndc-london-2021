using Microsoft.Azure.Cosmos.Table;

namespace Pizza.Data
{
    internal static class TableResultExtensions
    {
        public static bool IsSuccessStatusCode(this TableResult result) =>
            result.HttpStatusCode >= 200 && result.HttpStatusCode < 400;
    }
}