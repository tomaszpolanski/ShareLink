using System.Threading;
using System.Threading.Tasks;
using Utilities.Functional;

namespace ShareLink.Services.Interfaces
{
    public interface IClipboardService
    {
        Task<Option<string>> GetTextAsync(CancellationToken token);
    }
}