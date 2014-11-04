using Windows.UI.Xaml.Controls;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using System;

namespace ShareLink.Views
{
    public sealed partial class SettingsPage : SettingsFlyout, IView
    {
        public SettingsPage()
        {
            InitializeComponent();
            var viewModel = DataContext as IFlyoutViewModel;
            if (viewModel != null)
            {
                viewModel.CloseFlyout =  Hide;
            }
        }
    }
}