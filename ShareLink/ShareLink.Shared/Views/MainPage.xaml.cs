namespace ShareLink.Views
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //private void Current_VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        //{
        //    if (e.Visible)
        //    {
        //        GetCopiedText();
        //        LinkBox.Focus(FocusState.Programmatic);
        //        LinkBox.SelectAll();
        //    }
        //}

        //private async void GetCopiedText()
        //{
        //    try
        //    {
        //        var text = await Clipboard.GetContent().GetTextAsync();
        //        if (!string.IsNullOrEmpty(text))
        //        {
        //            LinkBox.Text = text;
        //            ShareButton.IsEnabled = Uri.IsWellFormedUriString(FormattedString, UriKind.Absolute);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}


        //private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        //{
        //    DataRequest request = e.Request;

        //    var uri = new Uri(FormattedString);
        //    request.Data.Properties.Title = "Share link";
        //    request.Data.Properties.Description = uri.Host;

        //    request.Data.SetWebLink(uri);
        //}


        //private void LinkBox_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    ShareButton.IsEnabled = Uri.IsWellFormedUriString(FormattedString, UriKind.Absolute);
        //}

        //private void LinkBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if (e.Key == VirtualKey.Enter)
        //    {
        //        DataTransferManager.ShowShareUI();
        //        e.Handled = true;
        //    }
        //}
    }
}