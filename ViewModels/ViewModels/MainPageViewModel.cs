using System;
using System.Reactive.Linq;
using Windows.System;
using Microsoft.Practices.Prism.Mvvm;
using Services.Interfaces;
using Utilities.Reactive;

namespace ShareLink.ViewModels.ViewModels
{
    public class MainPageViewModel : ViewModel, IDisposable
    {
        public ReadonlyReactiveProperty<string> SelectAllTextTrigger { get; private set; }

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

            _shareLinkSubscription = formattedStringObservable.Sample(ShareCommand.Merge(enterPressedObservable))
                                                              .Select(url => Observable.FromAsync(token => httpService.GetPageTitleAsync(new Uri(url), token))
                                                                                       .Select(title => new {Title = title, Url = url}))
                                                              .Switch()
                                                              .ObserveOnUI()
                                                              .Subscribe(shareData => ShareLink(dataTransferService, shareData.Title, shareData.Url));

        }

        public void Dispose()
        {
            _shareLinkSubscription.Dispose();
            ShareCommand.Dispose();
            Text.Dispose();
            KeyPressedCommand.Dispose();
            SelectAllTextTrigger.Dispose();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            return (text.StartsWith("http://") ? string.Empty : "http://") + text.Trim();
        }

        private static void ShareLink(IDataTransferService transferService, string title, string url)
        {
            transferService.Share(title, url, new Uri(url));
        }
    }
}