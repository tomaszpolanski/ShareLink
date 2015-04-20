using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Services.Interfaces;
using ShareLink.Services.Interfaces;
using Utilities.Functional;

namespace ShareLink.Services.Windows
{
    public class ClipboardService : IClipboardService
    {
        public async Task<Option<string>> GetTextAsync(CancellationToken token)
        {
            try
            {
                return Option<string>.AsOption(await Clipboard.GetContent().GetTextAsync().AsTask(token));
            }
            catch (Exception)
            {
                return Option<string>.None;
            }
        }
    }
}