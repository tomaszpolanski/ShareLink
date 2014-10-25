using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace ShareLink.Behaviors
{
    public class AnimationBehavior : Behavior<FrameworkElement>
    {
        public bool ChangedProperty
        {
            get { return (bool)GetValue(ChangedPropertyProperty); }
            set { SetValue(ChangedPropertyProperty, value); }
        }

        public static DependencyProperty ChangedPropertyProperty =
            DependencyProperty.RegisterAttached("ChangedProperty", typeof(bool), typeof(AnimationBehavior), new PropertyMetadata(null, OnPropertyChanged));

        public Storyboard Animation
        {
            get { return (Storyboard)GetValue(AnimationProperty); }
            set { SetValue(AnimationProperty, value); }
        }

        public static DependencyProperty AnimationProperty =
            DependencyProperty.RegisterAttached("Animation", typeof(Storyboard), typeof(AnimationBehavior), new PropertyMetadata(null));

        public static void SetAnimation(DependencyObject attached, Storyboard value)
        {
            attached.SetValue(AnimationProperty, value);
        }

        public static Storyboard GetAnimation(DependencyObject attached)
        {
            return (attached.GetValue(AnimationProperty) as Storyboard);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args == null || args.NewValue == null || !(bool)args.NewValue)
            {
                return;
            }
            var animation = GetAnimation(d);
            if (animation != null)
            {
                animation.Stop();
                var associatedObject = ((Behavior<FrameworkElement>)d).AssociatedObject;
                foreach(var timeLine in animation.Children)
                {
                    Storyboard.SetTarget(timeLine, associatedObject);
                }
                animation.Begin();
            }
        }

        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }
    }
}