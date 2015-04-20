using ShareLink.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Utilities.Functional;

namespace ShareLink.Services
{
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _client;

        public HttpClient()
        { 
            _client = new System.Net.Http.HttpClient();
        }

        public async Task<Option<string>> GetStringAsync(Uri uri, System.Threading.CancellationToken token)
        {
            using(var response = await _client.GetAsync(uri, token))
            {
                if (response.IsSuccessStatusCode)
                {
                    return Option<string>.AsOption( await response.Content.ReadAsStringAsync());
                }
                return Option<string>.None;
            }
        }
    }
}
