using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using ShareLink.Services;
using Utilities.Reactive;

namespace ShareLink.ViewModels.ViewModels
{
    public class SettingsPageViewModel : ViewModel, IFlyoutViewModel, IDisposable
    {
        public ReactiveProperty<bool> IsSpeechEnabled { get; private set; }

        private readonly IDisposable _isSpeechEnabledSubscription;
        private Action _closeFlyout;

        public Action CloseFlyout
        {
            get { return _closeFlyout; }
            set { SetProperty(ref _closeFlyout, value); }
        }

        public SettingsPageViewModel(ApplicationSettingsService settingsService)
        {
            IsSpeechEnabled = settingsService.IsSpeechEnabledObservable.ToReactiveProperty(); 
            _isSpeechEnabledSubscription = IsSpeechEnabled.Subscribe(isSpeechEnabled => settingsService.IsSpeechEnabled = isSpeechEnabled);

        }

        public void Dispose()
        {
            _isSpeechEnabledSubscription.Dispose();
            IsSpeechEnabled.Dispose();
        }

        public override void OnNavigatedFrom(Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatedFrom(viewModelState, suspending);
            Dispose();
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);
        }
    }
}