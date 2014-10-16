using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reflection;
using Windows.System;
using Microsoft.Practices.Prism.Mvvm;
using Services.Interfaces;
using ShareLink.Models;
using Utilities.Reactive;

namespace ShareLink.ViewModels.ViewModels
{
    public class MainPageViewModel : ViewModel, IDisposable
    {
        public ReadonlyReactiveProperty<string> SelectAllTextTrigger { get; private set; }
        public ReadonlyReactiveProperty<bool> IsInProgress { get; private set; }
        public ReadonlyReactiveProperty<string> ErrorMessage { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }
        public ReactiveCommand ShareCommand { get; private set; }
        public ReactiveCommand<VirtualKey?> KeyPressedCommand { get; private set; }

        private readonly IDisposable _shareLinkSubscription;

        public MainPageViewModel(IWindowService windowService, IDataTransferService dataTransferService, IClipboardService clipboardService, IHttpService httpService)
        {
            var clipboardChangedObservable = windowService.IsVisibleObservable.Select(isVisible => 
                                                                                      isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) : 
                                                                                                Observable.Empty<string>())
                                                                    .Switch()
                                                                    .Where(clipboardText => !string.IsNullOrEmpty(clipboardText));

            Text = clipboardChangedObservable.ToReactiveProperty();

            SelectAllTextTrigger = clipboardChangedObservable.Delay(TimeSpan.FromMilliseconds(300))
                                                       .ToReadonlyReactiveProperty(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

            var formattedStringObservable = Text.WhereIsNotNull()
                                                .Select(AddPrefixIfNeeded);

            var validLinkObservable = formattedStringObservable.Select(text => Uri.IsWellFormedUriString(text, UriKind.Absolute))
                                                               .DistinctUntilChanged();

            ShareCommand = validLinkObservable.ToReactiveCommand();
            KeyPressedCommand = validLinkObservable.ToReactiveCommand<VirtualKey?>();

            var enterPressedObservable = KeyPressedCommand.Where(args => args.Value == VirtualKey.Enter)
                                                          .SelectNull();

            var shareTrigger = formattedStringObservable.Select(text => ShareCommand.Merge(enterPressedObservable)
                                                                                                .Select(_ => text))
                                                                    .Switch()
                                                                    .Publish()
                                                                    .RefCount();
            var urlTitleResolveObservable = shareTrigger.Select(url => Observable.FromAsync(token => httpService.GetPageTitleAsync(new Uri(url), token))
                                                                                             .Select(title => new ShareData(title, url))
                                                                                             .Catch<ShareData, HttpRequestException>(exception => Observable.Return(new ShareData("Unknown", url, exception))))
                                                                    .Switch()
                                                                    .Publish()
                                                                    .RefCount();

            IsInProgress = shareTrigger.Select(_ => true)
                                                   .Merge(urlTitleResolveObservable.Select(_ => false))
                                                   .ToReadonlyReactiveProperty();

            ErrorMessage = shareTrigger.Select(_ => string.Empty)
                                       .Merge(urlTitleResolveObservable.Where(shareData => shareData.Exception != null)
                                                                       .Select(_ => "Couldn't resolve page title"))    
                                       .ToReadonlyReactiveProperty(String.Empty);

            _shareLinkSubscription = urlTitleResolveObservable.ObserveOnUI()
                                                                    
                                                              .Subscribe(shareData => ShareLink(dataTransferService, shareData.Title, shareData.Uri));

        }

        public void Dispose()
        {
            _shareLinkSubscription.Dispose();
            ShareCommand.Dispose();
            Text.Dispose();
            KeyPressedCommand.Dispose();
            SelectAllTextTrigger.Dispose();
            IsInProgress.Dispose();
            ErrorMessage.Dispose();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            return (text.StartsWith("http://") ? string.Empty : "http://") + text.Trim();
        }

        private static void ShareLink(IDataTransferService transferService, string title, Uri uri)
        {
            transferService.Share(title, uri.ToString(), uri);
        }
    }
}