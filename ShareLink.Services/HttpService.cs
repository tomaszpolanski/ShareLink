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

//        String htmlText = String htnlText = "<meta name=\"description\" content=\"Create 360 degree rotation product presentation online with 3Dbin. 360 product pics, object rotationg presentation can be created for your website at 3DBin.com web service.\" /><meta name=\"robots\" content=\"index, follow\" /><meta name=\"verify-v1\" content=\"x42ckCSDiernwyVbSdBDlxN0x9AgHmZz312zpWWtMf4=\" /><link rel=\"shortcut icon\" href=\"http://3dbin.com/favicon.ico\" type=\"image/x-icon\" /><link rel=\"shortcut icon\" href=\"http://anotherURL/someicofile.ico\" type=\"image/x-icon\">just to make sure it works with different link ending</link><link rel=\"stylesheet\" type=\"text/css\" href=\"http://3dbin.com/css/1261391049/style.min.css\" /><!--[if lt IE 8]>    <script src=\"http://3dbin.com/js/1261039165/IE8.js\" type=\"text/javascript\"></script><![endif]-->";

//foreach (Match match in Regex.Matches(htmlText, "<link .*? href=\"(.*?.ico)\""))
//{
//    String url = match.Groups[1].Value;

//    Console.WriteLine(url); // TEST this out
//}
    }
}