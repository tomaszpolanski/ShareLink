using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Services.Interfaces;

namespace Services
{
    public class HttpService : IHttpService
    {
        public async Task<string> GetPageTitleAsync(Uri uri, CancellationToken token)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var stringResponse = await response.Content.ReadAsStringAsync();
                string title = Regex.Match(stringResponse, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                return title;
            }
            return null;
        }
    }
}