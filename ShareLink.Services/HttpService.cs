using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ShareLink.Services.Interfaces;
using Utilities.Functional;

namespace ShareLink.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClient _client;

        public HttpService(IHttpClient client)
        {
            _client = client;
        }

        public async Task<Option<string>> GetPageTitleAsync(Uri uri, CancellationToken token)
        {
            return (await _client.GetStringAsync(uri, token)).Select(GetPageTitle);
        }

        private static string GetPageTitle(string pageContent)
        {
            return Regex.Match(pageContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        }
    }
}