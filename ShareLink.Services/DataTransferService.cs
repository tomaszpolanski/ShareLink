using System;
using System.Reactive.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using ShareLink.Models;
using ShareLink.Services.Interfaces;

namespace ShareLink.Services
{
    public class DataTransferService : IDataTransferService, IDisposable
    {
        private readonly IShareDataRepository _shareDataRepository;
        private readonly IDisposable _dataTransferSubscription;
        private readonly IDisposable _dataSharedSubscription;

        private ShareData _shareData;

        public IObservable<string> ShareTargetObservable { get; private set; } 

        public DataTransferService(IShareDataRepository shareDataRepository)
        {
            _shareDataRepository = shareDataRepository;
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            var sharingDataObservable = Observable.FromEventPattern<TypedEventHandler<DataTransferManager, DataRequestedEventArgs>, DataTransferManager, DataRequestedEventArgs>(
                                                       h => dataTransferManager.DataRequested += h,
                                                       h => dataTransferManager.DataRequested -= h)
                                                  .Select(ev => ev.EventArgs.Request.Data)
                                                  .Where(_ => _shareData != null);
            _dataTransferSubscription = sharingDataObservable.Subscribe(ShareTextHandler);

   


            ShareTargetObservable = Observable.FromEventPattern<TypedEventHandler<DataTransferManager, TargetApplicationChosenEventArgs>, DataTransferManager, TargetApplicationChosenEventArgs>(
                                                       h => dataTransferManager.TargetApplicationChosen += h,
                                                       h => dataTransferManager.TargetApplicationChosen -= h)
                                              .Select(ev => ev.EventArgs.ApplicationName);

            _dataSharedSubscription = sharingDataObservable.Select(shareData => ShareTargetObservable.Select(applicationName => new { ShareData = _shareData, Application = applicationName }))
                .Switch()
                .Subscribe(sharedData => HandleSharedData(shareDataRepository, sharedData.ShareData, sharedData.Application));
        }

        public void Dispose()
        {
            _dataTransferSubscription.Dispose();
            _dataSharedSubscription.Dispose();
        }

        public void Share(ShareData shareData)
        {
            _shareData = shareData;
            DataTransferManager.ShowShareUI();
        }


        private void ShareTextHandler(DataPackage data)
        {
                data.Properties.Title = _shareData.Title;
                data.Properties.Description = _shareData.Uri.ToString();
                data.SetWebLink(_shareData.Uri);
        }

        private static void HandleSharedData(IShareDataRepository repository, ShareData data, string applicationName)
        {
            data.ApplicationName = applicationName;
            repository.Add(data);
        }

    }
}