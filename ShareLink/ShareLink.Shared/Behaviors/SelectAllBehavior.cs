using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ShareLink.Behaviors
{
    public class SelectAllBehavior : Behavior<TextBox>
    {
        public object ChangedProperty
        {
            get { return (object)GetValue(ChangedPropertyProperty); }
            set { SetValue(ChangedPropertyProperty, value); }
        }

        public static DependencyProperty ChangedPropertyProperty =
            DependencyProperty.RegisterAttached("ChangedProperty", typeof(object), typeof(SelectAllBehavior), new PropertyMetadata(null, OnPropertyErrorsChanged));

        private static void OnPropertyErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args == null || args.NewValue == null)
            {
                return;
            }

            var control = ((Behavior<TextBox>)d).AssociatedObject;
            control.Focus(FocusState.Programmatic);
            control.SelectAll();
        }

        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }
    }
}
