using System;
using System.Reflection;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace ShareLink.Behaviors
{
    public abstract class Behavior<T> : DependencyObject, IBehavior where T : DependencyObject
    {
        public T AssociatedObject { get; private set; }

        DependencyObject IBehavior.AssociatedObject
        {
            get { return AssociatedObject; }
        }

        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject != null &&
                !typeof (T).GetTypeInfo().IsAssignableFrom(associatedObject.GetType().GetTypeInfo()))
                throw new Exception(string.Format("associatedObject is not assignable to type:", typeof (T)));

            AssociatedObject = associatedObject as T;
            OnAttached();
        }

        public void Detach()
        {
            OnDetached();
            AssociatedObject = null;
        }

        protected abstract void OnAttached();
        protected abstract void OnDetached();
    }
}