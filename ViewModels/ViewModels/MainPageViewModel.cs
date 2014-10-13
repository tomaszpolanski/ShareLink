using System;
using System.Reactive.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Services.Interfaces;
using Utilities.Reactive;

namespace ShareLink.ViewModels.ViewModels
{
    public class MainPageViewModel : ViewModel, IDisposable
    {
        private readonly ReadonlyReactiveProperty<string> _formattedString;
        private readonly ReadonlyReactiveProperty<bool> _isValid;

        private readonly IDisposable _updateSubscription;
        private readonly IDisposable _clipboardTextSubscription;
        public ReadonlyReactiveProperty<bool> UpdateProperty { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }
        public DelegateCommand ShareCommand { get; private set; }

        public MainPageViewModel(IWindowService windowService, IDataTransferService dataTransferService, IClipboardService clipboardService)
        {
            var isVisibleObservable = windowService.IsVisible;
            Text = new ReactiveProperty<string>(string.Empty);
            _formattedString = Text.Select(AddPrefixIfNeeded).ToReadonlyReactiveProperty();

            _isValid = _formattedString.Select(text => Uri.IsWellFormedUriString(text, UriKind.Absolute))
                .DistinctUntilChanged()
                .ToReadonlyReactiveProperty();

            var clipboardTextSubscription = isVisibleObservable.Select(
                    isVisible =>
                        isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) : Observable.Empty<string>())
                .Switch()
                .Where(clipboardText => !string.IsNullOrEmpty(clipboardText));
            UpdateProperty =
                Text.Sample(isVisibleObservable.Select(_ => string.Empty))
                .Select(_ => UpdateProperty != null && !UpdateProperty.Value)
                    .ToReadonlyReactiveProperty();
            _clipboardTextSubscription = clipboardTextSubscription
                .Subscribe(text => Text.Value = text);

            ShareCommand =
                new DelegateCommand(
                    () =>
                        dataTransferService.Share("Share link", _formattedString.Value, new Uri(_formattedString.Value)),
                    () => _isValid.Value);

            _updateSubscription = _isValid.Subscribe(_ => ShareCommand.RaiseCanExecuteChanged());
        }

        public void Dispose()
        {
            _updateSubscription.Dispose();
            _formattedString.Dispose();
            _isValid.Dispose();
            _clipboardTextSubscription.Dispose();
            Text.Dispose();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            return (text.StartsWith("http://") ? string.Empty : "http://") + text.Trim();
        }
    }
}