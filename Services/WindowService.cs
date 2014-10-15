using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Services.Interfaces;
using Utilities.Reactive;

namespace Services
{
    public class WindowService : IWindowService
    {
        private readonly Window _window;

        public bool IsVisible { get { return _window.Visible; } }

        public IObservable<bool> IsVisibleObservable { get; private set; } 

        public WindowService()
        {
            _window = Window.Current;
            IsVisibleObservable = DefineIsVisibleObservable(_window).ToReadonlyReactiveProperty();
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