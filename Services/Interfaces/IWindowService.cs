using Utilities.Reactive;

namespace Services.Interfaces
{
    public interface IWindowService
    {
        ReadonlyReactiveProperty<bool> IsVisible { get; }
    }
}