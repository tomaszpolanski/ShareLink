using System.Threading;
using System.Threading.Tasks;

namespace ShareLink.Services.Interfaces
{
    public interface ITextToSpeechService
    {
        Task PlayTextAsync(string text, CancellationToken token);
    }
}