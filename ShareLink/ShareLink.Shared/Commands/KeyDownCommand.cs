using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml;

namespace ShareLink.Commands
{
    public class KeyDownCommand
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] 
        private static readonly DependencyProperty
            CommandProperty =
                DependencyProperty.RegisterAttached("Command",
                    typeof (ICommand),
                    typeof (KeyDownCommand),
                    new PropertyMetadata(null, CommandPropertyChanged));

        public static void SetCommand(DependencyObject attached, ICommand value)
        {
            attached.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject attached)
        {
            return (attached.GetValue(CommandProperty) as ICommand);
        }

        private static void CommandPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs a)
        {
            var item = obj as FrameworkElement;
            if (item != null)
            {
                item.KeyUp += (sender, args) =>
                {
                    ExecuteCommand(item, new VirtualKey?(args.Key));
                    args.Handled = false;
                };
            }
        }

        private static void ExecuteCommand(DependencyObject attached, object argument)
        {
            var command = GetCommand(attached);
            if (command != null)
            {
                command.Execute(argument);
            }
        }
    }
}