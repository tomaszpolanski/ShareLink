using System;
using System.Reactive.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Services.Interfaces;
using Utilities.Reactive;

namespace Services
{
    public class WindowService : IWindowService
    {
        public ReadonlyReactiveProperty<bool> IsVisible { get; private set; }

        public WindowService()
        {
            IsVisible = DefineIsVisibleObservable(Window.Current).ToReadonlyReactiveProperty();
        }

        private static IObservable<bool> DefineIsVisibleObservable(Window window)
        {
            return Observable.FromEventPattern<WindowVisibilityChangedEventHandler, VisibilityChangedEventArgs>(h => window.VisibilityChanged += h,
                h => window.VisibilityChanged -= h)
                            .Select(ev => ev.EventArgs.Visible)
                            .StartWith(window.Visible);
        }
    }
}