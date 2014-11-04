using System;
namespace ShareLink.Controls
{
    public sealed partial class SettingsControl
    {
        public SettingsControl()
        {
            InitializeComponent();
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IDisposable viewModel = DataContext as IDisposable;
            if (viewModel != null)
            {
                viewModel.Dispose();
            }
        }
    }
}
