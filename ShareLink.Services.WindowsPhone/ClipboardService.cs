using System.Threading;
using System.Threading.Tasks;
using ShareLink.Services.Interfaces;
using Utilities.Functional;

namespace ShareLink.Services.WindowsPhone
{
    public class ClipboardService : IClipboardService
    {
        public Task<Option<string>> GetTextAsync(CancellationToken token)
        {
            return Task.FromResult(Option<string>.None);
        }
    }
}