using System;

namespace Services.Interfaces
{
    public interface IWindowService
    {
        IObservable<bool> IsVisibleObservable { get; } 
    }
}