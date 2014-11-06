using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace ShareLink.Controls
{
    public sealed class PageContainer : Control
    {
        public ICommand GoBackCommand
        {
            get { return (ICommand)GetValue(GoBackCommandProperty); }
            set { SetValue(GoBackCommandProperty, value); }
        }

        public static readonly DependencyProperty GoBackCommandProperty =
            DependencyProperty.Register(
                "GoBackCommand",
                typeof(ICommand),
                typeof(PageContainer),  
                new PropertyMetadata(null)
            );

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(PageContainer),
                new PropertyMetadata(null)
            );

        public FrameworkElement Body
        {
            get { return (FrameworkElement)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register(
                "Body",
                typeof(FrameworkElement),
                typeof(PageContainer),
                new PropertyMetadata(null)
            );

        public PageContainer()
        {
            DefaultStyleKey = typeof(PageContainer);
        }
    }
}
