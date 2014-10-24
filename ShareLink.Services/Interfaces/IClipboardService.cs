using System.Threading;
using System.Threading.Tasks;

namespace ShareLink.Services.Interfaces
{
    public interface IClipboardService
    {
        Task<string> GetTextAsync(CancellationToken token);
    }
}