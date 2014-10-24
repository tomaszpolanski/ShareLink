using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLink.Models;
using ShareLink.Services.Interfaces;
using Utilities.Reactive;

namespace ShareLink.Services
{
    public class ShareDataRepository : IShareDataRepository, IDisposable
    {

        private const string CacheFileName = "ShareDataRepository";

        private readonly ICacheService _cacheService;
        private readonly Subject<ShareData> _addedDataSubject =  new Subject<ShareData>();
        private readonly IConnectableObservable<ShareData> _shareDataObservable;
        private readonly IDisposable _shareDataSubscription;
        private readonly Collection<ShareData> _sharedData = new Collection<ShareData>();
        private readonly IDisposable _updatedSubscription;

        public IObservable<ShareData> ShareDataObservable { get { return _shareDataObservable; }}

        public ShareDataRepository(ICacheService cacheService)
        {
            _cacheService = cacheService;
            _shareDataObservable = DefineShareDataObservable(cacheService).Concat(_addedDataSubject).Replay();
            _shareDataSubscription = _shareDataObservable.Connect();
            _updatedSubscription = ShareDataObservable.Subscribe(AddShareData);
        }

        public void Dispose()
        {
            _shareDataSubscription.Dispose();
        }

        private void AddShareData(ShareData data)
        {
            _sharedData.Add(data);
        }

        private static IObservable<ShareData> DefineShareDataObservable(ICacheService service)
        {
            return Observable.FromAsync(_ => GetShareDataAsync(service, CancellationToken.None))
                .WhereIsNotNull()
                .Select(shareDataList => shareDataList.ToObservable())
                .Switch()
                .Publish()
                .RefCount();
        }

        private static async Task<ICollection<ShareData>> GetShareDataAsync(ICacheService service, CancellationToken token)
        {


            try
            {
                // Retrieve the items from the cache
                var data = await service.GetDataAsync<ICollection<ShareData>>(CacheFileName, token);
                return data;
            }
            catch (FileNotFoundException)
            {
            }

            return null;

        }

        public void Add(ShareData shareData)
        {
            _addedDataSubject.OnNext(shareData);
            _cacheService.SaveDataAsync(CacheFileName, _sharedData, CancellationToken.None);
        }
    }
}
