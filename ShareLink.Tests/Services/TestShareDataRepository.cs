using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLink.Services;
using ShareLink.Services.Interfaces;
using ShareLink.Models;

namespace ShareLink.Tests.Services
{
    [TestClass]
    public class TestShareDataRepository : ReactiveTest
    {
        private ICacheService _cacheService;

        [TestInitialize]
        public void Initialize()
        {
            _cacheService = A.Fake<ICacheService>();
        }

        private ShareDataRepository CreateService()
        {
            return new ShareDataRepository(_cacheService);
        }

        [TestMethod]
        public void ShareDataInStreamIsEmptyByDefault()
        {
            var scheduler = new TestScheduler();
            A.CallTo(() => _cacheService.GetDataAsync<ICollection<ShareData>>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult<ICollection<ShareData>>(new Collection<ShareData>()));
            var repository = CreateService();

            var actual = scheduler.Start(
                () => repository.ShareDataObservable, 0, 10, 100
                );

            var expected = new List<Recorded<Notification<ShareData>>>();

            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [TestMethod]
        public void CacheReturnsSingleSavedSharedData()
        {
            var scheduler = new TestScheduler();
            var shareData = new ShareData(string.Empty, null, string.Empty);
            A.CallTo(() => _cacheService.GetDataAsync<ICollection<ShareData>>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult<ICollection<ShareData>>(new Collection<ShareData>{shareData}));
            var repository = CreateService();

            var actual = scheduler.Start(
                () => repository.ShareDataObservable, 0, 10, 1000
                );

            var expected = new[] { OnNext(10, shareData) };

            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [TestMethod]
        public void CacheReturnsMultipleSavedSharedData()
        {
            var scheduler = new TestScheduler();
            var shareData1 = new ShareData(string.Empty, null, string.Empty);
            var shareData2 = new ShareData(string.Empty, null, string.Empty);
            var shareData3 = new ShareData(string.Empty, null, string.Empty);
            A.CallTo(() => _cacheService.GetDataAsync<ICollection<ShareData>>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult<ICollection<ShareData>>(new Collection<ShareData> { shareData1, shareData2, shareData3 }));
            var repository = CreateService();

            var actual = scheduler.Start(
                () => repository.ShareDataObservable, 0, 10, 1000
                );

            var expected = new[]
            {
                OnNext(10, shareData1),
                OnNext(10, shareData2),
                OnNext(10, shareData3)
            };

            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }

        [TestMethod]
        public void MultipleSubscriptionsRetriggerSharedData()
        {
            var scheduler = new TestScheduler();
            var shareData1 = new ShareData(string.Empty, null, string.Empty);
            var shareData2 = new ShareData(string.Empty, null, string.Empty);
            var shareData3 = new ShareData(string.Empty, null, string.Empty);
            A.CallTo(() => _cacheService.GetDataAsync<ICollection<ShareData>>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult<ICollection<ShareData>>(new Collection<ShareData> { shareData1, shareData2, shareData3 }));
            var repository = CreateService();

            scheduler.Start(
                () => repository.ShareDataObservable, 0, 10, 1000
                );
            var actual2 = scheduler.Start(
                () => repository.ShareDataObservable, 1000, 1010, 2000
                );

            var expected = new[]
            {
                OnNext(1010, shareData1),
                OnNext(1010, shareData2),
                OnNext(1010, shareData3)
            };

            ReactiveAssert.AreElementsEqual(expected, actual2.Messages);
        }

        [TestMethod]
        public void AddingNewDataUpdatesStream()
        {
            var scheduler = new TestScheduler();
            var shareData1 = new ShareData(string.Empty, null, string.Empty);
            var shareData2 = new ShareData(string.Empty, null, string.Empty);
            A.CallTo(() => _cacheService.GetDataAsync<ICollection<ShareData>>(A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult<ICollection<ShareData>>(new Collection<ShareData> { shareData1}));
            var repository = CreateService();

            repository.Add(shareData2);
            var actual = scheduler.Start(
                () => repository.ShareDataObservable, 0, 10, 1000
                );

            var expected = new[]
            {
                OnNext(10, shareData1),
                OnNext(10, shareData2)
            };

            ReactiveAssert.AreElementsEqual(expected, actual.Messages);
        }
    }
}