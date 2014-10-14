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
        private readonly ReadonlyReactiveProperty<string> _formattedString;
        public ReadonlyReactiveProperty<bool> UpdateProperty { get; private set; }

        public ReactiveProperty<string> Text { get; private set; }
        public ReactiveCommand ShareCommand { get; private set; }
        public ICommand KeyPressedCommand { get; private set; }

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

            ShareCommand = _formattedString.Select(text => Uri.IsWellFormedUriString(text, UriKind.Absolute))
                                           .DistinctUntilChanged()
                                           .ToReactiveCommand(_ => dataTransferService.Share("Share link", _formattedString.Value, new Uri(_formattedString.Value)));


            var enterPressed = new Subject<bool>();

            KeyPressedCommand = new DelegateCommand<VirtualKey?>(args => enterPressed.OnNext(args != null && args.Value == VirtualKey.Enter));

            enterPressed.Where(isEnterKey => isEnterKey)
                .Subscribe(_ => ShareCommand.Execute());
        }

        public void Dispose()
        {
            _formattedString.Dispose();
            ShareCommand.Dispose();
            Text.Dispose();
            UpdateProperty.Dispose();
        }

        private static string AddPrefixIfNeeded(string text)
        {
            return (text.StartsWith("http://") ? string.Empty : "http://") + text.Trim();
        }
    }
}