﻿using System;
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
using Utilities.Functional;
using System.Reactive;

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

        public MainPageViewModel(IWindowService windowService, 
                                 IDataTransferService dataTransferService, 
                                 IClipboardService clipboardService, 
                                 IHttpService httpService, 
                                 ISchedulerProvider schedulerProvider,
                                 ITextToSpeechService textToSpeechService,
                                 ApplicationSettingsService settingsService,
                                 ISettingsService settingsUiService,
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
                                       .ToReadonlyReactiveProperty();

            _textToSpeechSubscription = DefineTextToSpeachObservable(urlTitleResolveObservable, settingsService, textToSpeechService)
                                                    .Subscribe();

            _shareLinkSubscription = urlTitleResolveObservable.ObserveOnUI()
                                                              .Subscribe(shareData => ShareLink(dataTransferService, shareData.Title, shareData.Uri));

            SettingsCommand = new DelegateCommand(settingsUiService.ShowSettings);
            HistoryCommand = new DelegateCommand(() => navigationService.Navigate("History", null));
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

        private static IObservable<string> DefineClipboardObservable(IObservable<bool> applicationVisibilityObservable, IClipboardService clipboardService)
        {
            return applicationVisibilityObservable.Select(isVisible => isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) :
                                                                                   Observable.Empty<Option<string>>())
                                                   .Switch()
                                                   .Where(clipboardTextOption => clipboardTextOption.IsSome)
                                                   .Select(clipboardText => clipboardText.Get());
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
                                                        .Select(titleOption => titleOption.Or(() => String.Empty))
                                                        .Select(title => new ShareData(title, url))
                                                        .Catch<ShareData, HttpRequestException>(exception => Observable.Return(new ShareData(string.Empty, url))))
                               .Switch()
                               .Publish()
                               .RefCount();
        }

        private static IObservable<System.Reactive.Unit> DefineEnterPressedObservable(IObservable<object> keyPressedObservable )
        {
            return keyPressedObservable.Cast<VirtualKey?>()
                                       .Where(args => args != null && args.Value == VirtualKey.Enter)
                                       .SelectUnit();
        }

        private static IObservable<string> DefineShareTrigger(IObservable<string> formattedTextObservable, IObservable<object> shareObservable, IObservable<System.Reactive.Unit> enterPressedObservable)
        {
            return formattedTextObservable.Select(text => shareObservable.SelectUnit()
                                                                         .Merge(enterPressedObservable)
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
                                         .Merge(shareFinishedObservable.Where(shareData => string.IsNullOrEmpty(shareData.Title))
                                                                       .Select(_ => "Couldn't resolve page title"));
        }

        private static IObservable<System.Reactive.Unit> DefineTextToSpeachObservable(IObservable<ShareData> shareFinishedObservable, ApplicationSettingsService settingsService, ITextToSpeechService textToSpeechService)
        {
            return shareFinishedObservable.Where(_ => settingsService.IsSpeechEnabled)
                                          .Where(shareData => !string.IsNullOrEmpty( shareData.Title))
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
            transferService.Share(new ShareData(title, uri.ToString()));
        }
    }
}