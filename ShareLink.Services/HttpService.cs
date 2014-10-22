using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Services.Interfaces;
using ShareLink.Models;
using System.Diagnostics;
using ShareLink.Services;
using ShareLink.Services.Interfaces;

namespace Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClient _client;

        public HttpService(IHttpClient client)
        {
            _client = client;
        }

        public async Task<HtmlPage> GetHtmlPageAsync(Uri uri, CancellationToken token)
        {
            var stringResponse = await _client.GetStringAsync(uri, token);
            if ( !string.IsNullOrEmpty( stringResponse))
            {
                HtmlPage page = new HtmlPage();
                page.Title = GetPageTitle(stringResponse);
                page.Icon = GetPageIcon(stringResponse, uri);
                return page;
            }
            return null;
        }

        private static string GetPageTitle(string pageContent)
        {
            return Regex.Match(pageContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        }

        private static Uri GetPageIcon(string pageContent, Uri pageUri)
        {
            foreach (Match match in Regex.Matches(pageContent, "<link .*? href=\"(.*?.png)\""))
            {
                String url = match.Groups[1].Value;

                return new Uri(pageUri + url);
            }
            return null;
        }
    }
}