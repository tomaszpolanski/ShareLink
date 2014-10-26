using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace ShareLink.Commands
{
    public static class ItemClickCommand
    {
        [SuppressMessage("Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] 
        public static readonly DependencyProperty
            CommandProperty =
                DependencyProperty.RegisterAttached("Command",
                    typeof (ICommand),
                    typeof (ItemClickCommand),
                    new PropertyMetadata(null, CommandPropertyChanged));

        public static void SetCommand(DependencyObject attached, ICommand value)
        {
            attached.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject attached)
        {
            return (attached.GetValue(CommandProperty) as ICommand);
        }

        private static void CommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // first check if the component is a list
            if (obj is ListViewBase)
            {
                var list = obj as ListViewBase;
                list.ItemClick += (sender, e) => ExecuteCommand(obj, e.ClickedItem);
            }
                // check if component has Tapped event
            else if (obj is UIElement)
            {
                var item = obj as UIElement;
                item.Tapped += (sender, e) => ExecuteCommand(obj, e);
            }
            else if (obj is Hyperlink)
            {
                var link = obj as Hyperlink;
                link.Click += (sender, e) => ExecuteCommand(obj, e);
            }
        }

        private static void ExecuteCommand(DependencyObject attached, object argument)
        {
            ICommand command = GetCommand(attached);
            if (command != null && command.CanExecute(argument))
            {
                command.Execute(argument);
            }
        }
    }
}