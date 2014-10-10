using System;
using Windows.ApplicationModel.DataTransfer;
using Services.Interfaces;

namespace Services
{
    public class DataTransferService : IDataTransferService, IDisposable
    {
        private readonly DataTransferManager _dataTransferManager;

        private string _title;
        private string _description;
        private Uri _webLink;

        public DataTransferService()
        {
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += ShareTextHandler;
        }

        public void Dispose()
        {
            _dataTransferManager.DataRequested -= ShareTextHandler;
        }

        public void Share(string title, string description, Uri webLink)
        {
            _title = title;
            _description = description;
            _webLink = webLink;
            DataTransferManager.ShowShareUI();
        }


        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;

            request.Data.Properties.Title = _title;
            request.Data.Properties.Description = _description;

            request.Data.SetWebLink(_webLink);
            _title = null;
            _description = null;
            _webLink = null;
        }

    }
}