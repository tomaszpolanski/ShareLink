using System.Threading;
using System.Threading.Tasks;
using ShareLink.Services.Interfaces;

namespace ShareLink.Services.WindowsPhone
{
    public class ClipboardService : IClipboardService
    {
        public Task<string> GetTextAsync(CancellationToken token)
        {
            return Task.FromResult<string>(null);
        }
    }
}