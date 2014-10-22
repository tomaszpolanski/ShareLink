using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Services.Interfaces;
using Windows.Storage.Streams;

namespace ShareLink.Services
{
    public class DataTransferService : IDataTransferService, IDisposable
    {
        private readonly IDisposable _dataTransferSubscription;

        private string _title;
        private string _description;
        private Uri _webLink;
        private Uri _icon;

        public DataTransferService()
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferSubscription = Observable.FromEventPattern<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, DataTransferManager, DataRequestedEventArgs>(
                                                       h => dataTransferManager.DataRequested += h,
                                                       h => dataTransferManager.DataRequested -= h)
                                                  .Select(ev => ev.EventArgs.Request.Data)
                                                  .Subscribe(ShareTextHandler);

        }

        public void Dispose()
        {
            _dataTransferSubscription.Dispose();
        }

        public void Share(string title, string description, Uri webLink, Uri icon)
        {
            _title = title;
            _description = description;
            _webLink = webLink;
            _icon = icon;
            Debug.WriteLine("Sharing: " + title);
            DataTransferManager.ShowShareUI();
        }


        private void ShareTextHandler(DataPackage data)
        {
            if (_title != null)
            {
                data.Properties.Title = _title;
                data.Properties.Description = _description;
                data.SetWebLink(_webLink);
                if (_icon != null)
                {
                    data.SetBitmap(RandomAccessStreamReference.CreateFromUri(_icon));
                }
            }
        }

    }
}