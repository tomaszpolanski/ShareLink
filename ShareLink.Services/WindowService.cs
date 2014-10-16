using System;
using System.Reactive.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Services.Interfaces;

namespace Services
{
    public class WindowService : IWindowService
    {
        private readonly Window _window;

        public IObservable<bool> IsVisibleObservable { get { return DefineIsVisibleObservable(_window); } } 

        public WindowService()
        {
            _window = Window.Current;
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