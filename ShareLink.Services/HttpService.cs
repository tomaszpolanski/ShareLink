using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Services.Interfaces;
using ShareLink.Models;
using System.Diagnostics;
using ShareLink.Services;

namespace Services
{
    public class HttpService : IHttpService
    {
        public async Task<string> GetPageTitleAsync(Uri uri, CancellationToken token)
        {
            var client = new HttpClient();
            var stringResponse = await client.GetStringAsync(uri, token);
            if ( string.IsNullOrEmpty( stringResponse))
            {
                return GetPageTitle(stringResponse);
            }
            return null;
        }

        private static string GetPageTitle(string pageContent)
        {
            return Regex.Match(pageContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
        }

        public HttpService()
        {
            String  htnlText = "<meta name=\"description\" content=\"This is page's content.\" /><meta name=\"robots\" content=\"index, follow\" /><meta name=\"verify-v1\" content=\"x42ckCSDiernwyVbSdBDlxN0x9AgHmZz312zpWWtMf4=\" /><link rel=\"shortcut icon\" href=\"http://3dbin.com/favicon.png\" type=\"image/x-icon\" /><link rel=\"shortcut icon\" href=\"http://anotherURL/someicofile.png\" type=\"image/x-icon\">just to make sure it works with different link ending</link><link rel=\"stylesheet\" type=\"text/css\" href=\"http://3dbin.com/css/1261391049/style.min.css\" />";
            GetPageIcon(htnlText);
        }

        public static string GetPageIcon(string pageContent)
        {
            foreach (Match match in Regex.Matches(pageContent, "<link .*? href=\"(.*?.png)\""))
            {
                String url = match.Groups[1].Value;

                Debug.WriteLine(url); 
            }
            return null;
        }


    }
}