using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShareLink.Services.Interfaces
{
    public interface IHttpService
    {
        Task<string> GetPageTitleAsync(Uri uri, CancellationToken token);
    }
}