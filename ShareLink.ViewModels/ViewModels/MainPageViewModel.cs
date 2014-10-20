using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Windows.Input;
using Windows.System;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Services.Interfaces;
using ShareLink.Models;
using ShareLink.Services;
using ShareLink.Services.Interfaces;
using Utilities.Reactive;

namespace ShareLink.ViewModels.ViewModels
{
    public class MainPageViewModel : ViewModel, IDisposable
    {
        public ReadonlyReactiveProperty<bool> SelectAllTextTrigger { get; private set; }
        public ReadonlyReactiveProperty<bool> IsInProgress { get; private set; }
        public ReadonlyReactiveProperty<string> ErrorMessage { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }
        public ReactiveCommand ShareCommand { get; private set; }
        public ReactiveCommand<object> KeyPressedCommand { get; private set; }
        public ICommand SettingsCommand { get; private set; }

        private readonly IDisposable _shareLinkSubscription;
        private readonly IDisposable _textToSpeechSubscription;

        public MainPageViewModel(IWindowService windowService, 
                                 IDataTransferService dataTransferService, 
                                 IClipboardService clipboardService, 
                                 IHttpService httpService, 
                                 ISchedulerProvider schedulerProvider,
                                 ITextToSpeechService textToSpeechService,
                                 ApplicationSettingsService settingsService,
                                 INavigationService navigationService)
        {
            var clipboardChangedObservable = windowService.IsVisibleObservable.Select(isVisible => 
                                                                                      isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) : 
                                                                                                Observable.Empty<string>())
                                                                    .Switch()
                                                                    .Where(clipboardText => !string.IsNullOrEmpty(clipboardText));

            Text = clipboardChangedObservable.ToReactiveProperty();

            SelectAllTextTrigger = windowService.IsVisibleObservable.Where(isVisible => isVisible)
                                                                    .Delay(TimeSpan.FromMilliseconds(300), schedulerProvider.Default)
                                                                    .ToReadonlyReactiveProperty(mode: ReactivePropertyMode.None);

            var formattedStringObservable = Text.WhereIsNotNull()
                                                .Select(AddPrefixIfNeeded);

            var validLinkObservable = formattedStringObservable.Select(text => Uri.IsWellFormedUriString(text, UriKind.Absolute))
                                                               .DistinctUntilChanged();

            ShareCommand = validLinkObservable.ToReactiveCommand();
            KeyPressedCommand = validLinkObservable.ToReactiveCommand<object>();

            var enterPressedObservable = KeyPressedCommand.Cast<VirtualKey?>()
                                                          .Where(args => args.Value == VirtualKey.Enter)
                                                          .SelectNull();

            var shareTrigger = formattedStringObservable.Select(text => ShareCommand.Merge(enterPressedObservable)
                                                                                                .Select(_ => text))
                                                                    .Switch()
                                                                    .Publish()
                                                                    .RefCount();
            var urlTitleResolveObservable = shareTrigger.Select(url => Observable.FromAsync(token => httpService.GetPageTitleAsync(new Uri(url), token))
                                                                                             .Select(title => new ShareData(title, url))
                                                                                             .Catch<ShareData, HttpRequestException>(exception => Observable.Return(new ShareData(url.ToString(), url, exception))))
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

            _textToSpeechSubscription = urlTitleResolveObservable.Where(_ => settingsService.IsSpeechEnabled)
                .Where(shareData => shareData.Exception == null)
                .SubscribeOnUI()
                .ObserveOnUI()
                .Select(shareData => Observable.FromAsync(token => textToSpeechService.PlayTextAsync(shareData.Title, token)))
                .Switch()
                
                .Subscribe();

            _shareLinkSubscription = urlTitleResolveObservable.ObserveOnUI()
                                                                    
                                                              .Subscribe(shareData => ShareLink(dataTransferService, shareData.Title, shareData.Uri));

            SettingsCommand = new DelegateCommand(() => navigationService.Navigate("Settings", null));
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
            _textToSpeechSubscription.Dispose();
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatedFrom(viewModelState, suspending);
            Dispose();
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