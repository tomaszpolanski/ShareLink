using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Functional;

namespace ShareLink.Services.Interfaces
{
    public interface IHttpService
    {
        Task<Option<string>> GetPageTitleAsync(Uri uri, CancellationToken token);
    }
}