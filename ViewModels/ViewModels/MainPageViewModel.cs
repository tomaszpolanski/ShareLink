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

        private readonly IDisposable _updateButtonSubscription;
        public ReadonlyReactiveProperty<bool> UpdateProperty { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }
        public DelegateCommand ShareCommand { get; private set; }

        public MainPageViewModel(IWindowService windowService, IDataTransferService dataTransferService, IClipboardService clipboardService)
        {
            var isVisibleObservable = windowService.IsVisible;

            var clipboardChangedObservable = isVisibleObservable.Select(
               isVisible => isVisible ? Observable.FromAsync(clipboardService.GetTextAsync) : Observable.Empty<string>())
                                                          .Switch()
                                                          .Where(clipboardText => !string.IsNullOrEmpty(clipboardText));

            Text = clipboardChangedObservable.ToReactiveProperty();

            UpdateProperty = clipboardChangedObservable.Delay(TimeSpan.FromMilliseconds(300))
                     .Select(_ => UpdateProperty != null && !UpdateProperty.Value)
                     .ToReadonlyReactiveProperty();

            _formattedString = Text.WhereIsNotNull()
                                   .Select(AddPrefixIfNeeded)
                                   .ToReadonlyReactiveProperty();

            _isValid = _formattedString.Select(text => Uri.IsWellFormedUriString(text, UriKind.Absolute))
                                       .DistinctUntilChanged()
                                       .ToReadonlyReactiveProperty();




            ShareCommand =
                new DelegateCommand(
                    () => dataTransferService.Share("Share link", _formattedString.Value, new Uri(_formattedString.Value)),
                    () => _isValid.Value);

            _updateButtonSubscription = _isValid.Subscribe(_ => ShareCommand.RaiseCanExecuteChanged());
        }

        public void Dispose()
        {
            _updateButtonSubscription.Dispose();
            _formattedString.Dispose();
            _isValid.Dispose();
            Text.Dispose();
            UpdateProperty.Dispose();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            return (text.StartsWith("http://") ? string.Empty : "http://") + text.Trim();
        }
    }
}