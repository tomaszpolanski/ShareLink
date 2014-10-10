// -----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="NOKIA">
// copyright © 2014 Nokia.  All rights reserved.
// This material, including documentation and any related computer
// programs, is protected by copyright controlled by Nokia.  All
// rights are reserved.  Copying, including reproducing, storing,
// adapting or translating, any or all of this material requires the
// prior written consent of Nokia.  This material also contains
// confidential information which may not be disclosed to others
// without the prior written consent of Nokia.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ShareLink
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string FormattedString
        {
            get { return (LinkBox.Text.StartsWith("http://") ? string.Empty : "http://") + LinkBox.Text.Trim(); }
        }

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareTextHandler;

            Window.Current.VisibilityChanged += Current_VisibilityChanged;
        }

        void Current_VisibilityChanged(object sender, Windows.UI.Core.VisibilityChangedEventArgs e)
        {
            if (e.Visible)
            {
                GetCopiedText();
                LinkBox.Focus(FocusState.Programmatic);
                LinkBox.SelectAll();
            }
        }

        private async void GetCopiedText()
        {
            try
            {
                var text = await Clipboard.GetContent().GetTextAsync();
                if (!string.IsNullOrEmpty(text))
                {
                    LinkBox.Text = text;
                    ShareButton.IsEnabled = Uri.IsWellFormedUriString(FormattedString, UriKind.Absolute);
                }
            }
            catch (Exception)
            {
            }
        }


        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;

            var uri = new Uri(FormattedString);
            request.Data.Properties.Title = "Tomek Share";
            request.Data.Properties.Description = uri.Host;

            request.Data.SetWebLink(uri);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void LinkBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ShareButton.IsEnabled = Uri.IsWellFormedUriString(FormattedString, UriKind.Absolute);
        }

        private void LinkBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                DataTransferManager.ShowShareUI();
                e.Handled = true;
            }
        }
    }
}