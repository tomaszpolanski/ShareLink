using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Functional;

namespace ShareLink.Services.Interfaces
{
    public interface IHttpClient
    {
        Task<Option<string>> GetStringAsync(Uri uri, CancellationToken token);
    }
}
