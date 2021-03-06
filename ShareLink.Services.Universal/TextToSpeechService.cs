﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ShareLink.Services.Interfaces;
using Utilities.Functional;

namespace ShareLink.Services.Universal
{
    public class TextToSpeechService : ITextToSpeechService
    {
        public async Task PlayTextAsync(string text, CancellationToken token)
        {
            try
            {
                using (var speech = new SpeechSynthesizer())
                {
                    using (var speechStream = await speech.SynthesizeTextToStreamAsync(text).AsTask(token))
                    {
                        await PlayStreamAsync(speechStream, speechStream.ContentType, token);
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
            var tcs = new TaskCompletionSource<Unit>();
            token.Register(tcs.SetCanceled);
            RoutedEventHandler successHandler = null;
            ExceptionRoutedEventHandler failedHandler = null;

            var soundPlayer = new MediaElement();
            soundPlayer.SetSource(stream, mimeType);

            successHandler = (a, s) => tcs.SetResult(Unit.Default);
            failedHandler = (a, s) => tcs.SetResult(Unit.Default);
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

    }
}