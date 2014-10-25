using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive.Concurrency;
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

        public ICommand HistoryCommand { get; private set; }

        private readonly IDisposable _shareLinkSubscription;
        private readonly IDisposable _textToSpeechSubscription;

        private IDisposable _test;

        public MainPageViewModel(IWindowService windowService, 
                                 IDataTransferService dataTransferService, 
                                 IClipboardService clipboardService, 
                                 IHttpService httpService, 
                                 ISchedulerProvider schedulerProvider,
                                 ITextToSpeechService textToSpeechService,
                                 ApplicationSettingsService settingsService,
                                 ISettingsService settingsUiService,
                                 IShareDataRepository shareDataRepository,
                                 INavigationService navigationService)
        {
            Text = DefineClipboardObservable(windowService.IsVisibleObservable, clipboardService).ToReactiveProperty();

            SelectAllTextTrigger = DefineSelectAllTextTriggerObservable(windowService.IsVisibleObservable, schedulerProvider.Default)
                                                                    .ToReadonlyReactiveProperty(mode: ReactivePropertyMode.None);

            var formattedStringObservable = DefineFormattedStringObservable(Text);

            var validLinkObservable = DefineValidUriObservable(formattedStringObservable);

            ShareCommand = validLinkObservable.ToReactiveCommand();
            KeyPressedCommand = validLinkObservable.ToReactiveCommand<object>();

            var enterPressedObservable = DefineEnterPressedObservable(KeyPressedCommand);

            var shareTrigger = DefineShareTrigger(formattedStringObservable, ShareCommand, enterPressedObservable);

            var urlTitleResolveObservable = DefineUrlTitleResolveObservable(shareTrigger, httpService);

            IsInProgress = DefineInProgressObservable(shareTrigger, urlTitleResolveObservable)
                                       .ToReadonlyReactiveProperty();

            ErrorMessage = DefineErrorMessageObservable(shareTrigger, urlTitleResolveObservable)
                                       .ToReadonlyReactiveProperty((string)null);

            _textToSpeechSubscription = DefineTextToSpeachObservable(urlTitleResolveObservable, settingsService, textToSpeechService)
                                                    .Subscribe();

            _shareLinkSubscription = urlTitleResolveObservable.ObserveOnUI()
                                                              .Subscribe(shareData => ShareLink(dataTransferService, shareData.Title, shareData.Uri));

            SettingsCommand = new DelegateCommand(settingsUiService.ShowSettings);
            HistoryCommand = new DelegateCommand(() => navigationService.Navigate("History", null));

            _test = shareDataRepository.ShareDataObservable.Subscribe(shareData => Debug.WriteLine(shareData));

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

        private static IObservable<string> DefineClipboardObservable(IObservable<bool> applicationVisibilityObservable, IClipboardService clipboardService)
        {
            return  applicationVisibilityObservable.Select(isVisible => isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) : 
                                                                                    Observable.Empty<string>())
                                                   .Switch()
                                                   .Where(clipboardText => !string.IsNullOrEmpty(clipboardText));
        }

        private static IObservable<bool> DefineSelectAllTextTriggerObservable(IObservable<bool> applicationVisibilityObservable, IScheduler timerScheduler)
        {
            return applicationVisibilityObservable.Where(isVisible => isVisible)
                                                  .Delay(TimeSpan.FromMilliseconds(300), timerScheduler);
        }

        private static IObservable<string> DefineFormattedStringObservable(IObservable<string> textObservable )
        {
            return textObservable.WhereIsNotNull()
                                 .Select(AddPrefixIfNeeded);
        }

        private static IObservable<bool> DefineValidUriObservable(IObservable<string> formattedTextObservable)
        {
            return formattedTextObservable.Select(text => Uri.IsWellFormedUriString(text, UriKind.Absolute))
                                          .DistinctUntilChanged();
        }

        private static IObservable<ShareData> DefineUrlTitleResolveObservable(IObservable<string> shareTrigger, IHttpService httpService )
        {
            return shareTrigger.Select(url => Observable.FromAsync(token => httpService.GetPageTitleAsync(new Uri(url), token))
                                                        .Select(title => new ShareData(title, url))
                                                        .Catch<ShareData, HttpRequestException>(exception => Observable.Return(new ShareData(url.ToString(), url, exception))))
                               .Switch()
                               .Publish()
                               .RefCount();
        }

        private static IObservable<object> DefineEnterPressedObservable(IObservable<object> keyPressedObservable )
        {
            return keyPressedObservable.Cast<VirtualKey?>()
                                       .Where(args => args != null && args.Value == VirtualKey.Enter)
                                       .SelectNull();
        }

        private static IObservable<string> DefineShareTrigger(IObservable<string> formattedTextObservable, IObservable<object> shareObservable, IObservable<object> enterPressedObservable)
        {
            return formattedTextObservable.Select(text => shareObservable.Merge(enterPressedObservable)
                                                                         .Select(_ => text))
                                          .Switch()
                                          .Publish()
                                          .RefCount();   
        }

        private static IObservable<bool> DefineInProgressObservable(IObservable<object> shareStartedObservable, IObservable<ShareData> shareFinishedObservable)
        {
            return shareStartedObservable.Select(_ => true)
                                         .Merge(shareFinishedObservable.Select(_ => false));
        }

        private static IObservable<string> DefineErrorMessageObservable(IObservable<object> shareStartedObservable, IObservable<ShareData> shareFinishedObservable)
        {
            return shareStartedObservable.Select(_ => (string)null)
                                         .Merge(shareFinishedObservable.Where(shareData => shareData.Exception != null)
                                                                       .Select(_ => "Couldn't resolve page title"));
        }

        private static IObservable<System.Reactive.Unit> DefineTextToSpeachObservable(IObservable<ShareData> shareFinishedObservable, ApplicationSettingsService settingsService, ITextToSpeechService textToSpeechService)
        {
            return shareFinishedObservable.Where(_ => settingsService.IsSpeechEnabled)
                                          .Where(shareData => shareData.Exception == null)
                                          .SubscribeOnUI()
                                          .ObserveOnUI()
                                          .Select(shareData => Observable.FromAsync(token => textToSpeechService.PlayTextAsync(shareData.Title, token)))
                                          .Switch();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            const string httpPrefix = "http://";
            return (text.StartsWith(httpPrefix) ? string.Empty : httpPrefix) + text.Trim();
        }

        private static void ShareLink(IDataTransferService transferService, string title, Uri uri)
        {
            transferService.Share(title, uri.ToString(), uri);
        }
    }
}