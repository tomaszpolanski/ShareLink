using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLink.Controls
{
    public class DisposingPage : VisualStateAwarePage
    {
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            IDisposable viewModel = DataContext as IDisposable;
            if (viewModel != null)
            {
                viewModel.Dispose();
            }
        }
    }
}
