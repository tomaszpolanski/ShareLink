using System;

namespace Services.Interfaces
{
    public interface IWindowService
    {
        bool IsVisible { get; }
        IObservable<bool> IsVisibleObservable { get; } 
    }
}