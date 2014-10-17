using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShareLink.Services.Interfaces;

namespace ShareLink.Services.Universal
{
    public class TextToSpeechService : ITextToSpeechService
    {
        public async Task PlayTextAsync(string text, CancellationToken token)
        {
            try
            {
                var voice = GetSpeechVoice();

                if (voice != null)
                {
                    using( var speech = new SpeechSynthesizer() {Voice = voice})
                    {
                        using( var speechStream = await speech.SynthesizeTextToStreamAsync(text).AsTask(token))
                        {
                            await PlayStreamAsync(speechStream, speechStream.ContentType, token);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
                throw e;
            }

        }


        private static async Task PlayStreamAsync(IRandomAccessStream stream, string mimeType, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>();
            token.Register(tcs.SetCanceled);
            RoutedEventHandler successHandler = null;
            ExceptionRoutedEventHandler failedHandler = null;

            var soundPlayer = new MediaElement();
            soundPlayer.SetSource(stream, mimeType);

            successHandler = (a, s) => tcs.SetResult(null);
            failedHandler = (a, s) => tcs.SetResult(null);
            soundPlayer.MediaEnded += successHandler;
            soundPlayer.MediaFailed += failedHandler;
            soundPlayer.Play();
            try
            {
                await tcs.Task;
            }
            catch (TaskCanceledException)
            {
                
            }
            soundPlayer.Stop();
        }


        private static VoiceInformation GetSpeechVoice()
        {
            string language = CultureInfo.CurrentCulture.ToString();
            var voices = SpeechSynthesizer.AllVoices.Where(v => v.Language == language);
            var voice = voices.FirstOrDefault(v => v.Gender == VoiceGender.Female);
            if (voice == null)
            {
                voice = voices.FirstOrDefault();
            }
            return voice;
        }
    }
}