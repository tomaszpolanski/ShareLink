using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Windows.System;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Services.Interfaces;
using Utilities.Reactive;

namespace ShareLink.ViewModels.ViewModels
{
    public class MainPageViewModel : ViewModel, IDisposable
    {
        public ReadonlyReactiveProperty<string> UpdateProperty { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }
        public ReactiveCommand ShareCommand { get; private set; }
        public ReactiveCommand<VirtualKey?> KeyPressedCommand { get; private set; }

        private readonly IDisposable _shareLinkSubscription;

        public MainPageViewModel(IWindowService windowService, IDataTransferService dataTransferService, IClipboardService clipboardService)
        {
            var clipboardChangedObservable = windowService.IsVisible.Select(isVisible => 
                                                                            isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) : 
                                                                                        Observable.Empty<string>())
                                                                    .Switch()
                                                                    .Where(clipboardText => !string.IsNullOrEmpty(clipboardText));

            Text = clipboardChangedObservable.ToReactiveProperty();

            UpdateProperty = clipboardChangedObservable.Delay(TimeSpan.FromMilliseconds(300))
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
                                                              .Subscribe(url => ShareLink(dataTransferService, url));

        }

        public void Dispose()
        {
            _shareLinkSubscription.Dispose();
            ShareCommand.Dispose();
            Text.Dispose();
            KeyPressedCommand.Dispose();
            UpdateProperty.Dispose();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            return (text.StartsWith("http://") ? string.Empty : "http://") + text.Trim();
        }

        private static void ShareLink(IDataTransferService transferService, string url)
        {
            transferService.Share("Share link", url, new Uri(url));
        }
    }
}