using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Services.Interfaces;

namespace ShareLink.Services.Windows
{
    public class ClipboardService : IClipboardService
    {
        public async Task<string> GetTextAsync(CancellationToken token)
        {
            try
            {
                return await Clipboard.GetContent().GetTextAsync().AsTask(token);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}