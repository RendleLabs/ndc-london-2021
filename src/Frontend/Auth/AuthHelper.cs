using System.Net.Http;
using System.Threading.Tasks;

namespace Frontend.Auth
{
    public class AuthHelper
    {
        private readonly HttpClient _client;
        private string _token;

        public AuthHelper(HttpClient client)
        {
            _client = client;
        }

        public ValueTask<string> GetTokenAsync()
        {
            if (_token is {Length: > 0})
            {
                return new ValueTask<string>(_token);
            }

            return new ValueTask<string>(GetTokenImpl());
        }

        private async Task<string> GetTokenImpl()
        {
            _token = await _client.GetStringAsync("/generateJwtToken?name=frontend");
            return _token;
        }
    }
}