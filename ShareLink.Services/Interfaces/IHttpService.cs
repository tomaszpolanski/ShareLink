using ShareLink.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IHttpService
    {
        Task<HtmlPage> GetHtmlPageAsync(Uri uri, CancellationToken token);
    }
}