using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ShareLink.Services.Interfaces;

namespace ShareLink.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClient _client;

        public HttpService(IHttpClient client)
        {
            _client = client;
        }

        public async Task<string> GetPageTitleAsync(Uri uri, CancellationToken token)
        {
            var stringResponse = await _client.GetStringAsync(uri, token);
            if ( !string.IsNullOrEmpty( stringResponse))
            {
                return GetPageTitle(stringResponse);

            }
            return null;
        }

        private static string GetPageTitle(string pageContent)
        {
            return Regex.Match(pageContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        }
    }
}