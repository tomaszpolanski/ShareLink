using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Interfaces;
using ShareLink.ViewModels.ViewModels;


namespace ShareLink.Tests.ViewModels
{

    [TestClass]
    public class TestMainPageViewModel
    {
        private IWindowService _windowService;
        private IDataTransferService _dataTransferService;
        private IClipboardService _clipboardService;
        private IHttpService _httpService;
        private ISchedulerProvider _schedulerProvider;
        private readonly TestScheduler _testScheduler = new TestScheduler();

        [TestInitialize]
        public void Initialize()
        {
            _windowService = A.Fake<IWindowService>();
            _dataTransferService = A.Fake<IDataTransferService>();
            _clipboardService = A.Fake<IClipboardService>();
            _httpService = A.Fake<IHttpService>();
            _schedulerProvider = A.Fake<ISchedulerProvider>();

            A.CallTo(() => _schedulerProvider.Default).Returns(_testScheduler);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        private MainPageViewModel CreateViewModel()
        {
            return new MainPageViewModel(_windowService, _dataTransferService, _clipboardService, _httpService, _schedulerProvider);
        }

        [TestMethod]
        public void DefaultTextValue()
        {
            var viewModel = CreateViewModel();

            Assert.AreEqual(null, viewModel.Text.Value);
        }

        [TestMethod]
        public void CopyClipboardTextWhenApplicationBecomesVisible()
        {
            const string clipboardText = "SomeText";
            var visibilitySubject = new Subject<bool>();
            A.CallTo(() => _clipboardService.GetTextAsync(A<CancellationToken>.Ignored)).Returns(Task.FromResult(clipboardText));
            A.CallTo(() => _windowService.IsVisibleObservable).Returns(visibilitySubject);
            var viewModel = CreateViewModel();

            visibilitySubject.OnNext(true);

            Assert.AreEqual(clipboardText, viewModel.Text.Value);
        }


        [TestMethod]
        public void SelectAllIsNotTriggeredByDefault()
        {
            var viewModel = CreateViewModel();
            bool selectAllWasTriggered = false;
            viewModel.SelectAllTextTrigger.Subscribe(_ => selectAllWasTriggered = true);

            _testScheduler.AdvanceBy(TimeSpan.FromDays(10).Ticks);

            Assert.IsFalse(selectAllWasTriggered);
        }

        [TestMethod]
        public void SelectAllIsNotTriggeredInstantlyAfterVisibilityChange()
        {
            var visibilitySubject = new Subject<bool>();
            A.CallTo(() => _windowService.IsVisibleObservable).Returns(visibilitySubject);
            var viewModel = CreateViewModel();
            bool selectAllWasTriggered = false;
            viewModel.SelectAllTextTrigger.Subscribe(_ => selectAllWasTriggered = true);

            visibilitySubject.OnNext(true);

            Assert.IsFalse(selectAllWasTriggered);
        }

        [TestMethod]
        public void TriggerTextSelectionAfterDelayWhenCopiedTextFromClipboard()
        {
            var visibilitySubject = new Subject<bool>();
            A.CallTo(() => _windowService.IsVisibleObservable).Returns(visibilitySubject);
            var viewModel = CreateViewModel();
            bool selectAllWasTriggered = false;
            viewModel.SelectAllTextTrigger.Subscribe(_ => selectAllWasTriggered = true);

            visibilitySubject.OnNext(true);
            _testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(3000).Ticks);

            Assert.IsTrue(selectAllWasTriggered);
        }

        [TestMethod]
        public void CannotExecuteShareCommandIfTextIsNotValidUri()
        {
            var viewModel = CreateViewModel();

            viewModel.Text.Value = "#$%^&*(";

            Assert.AreEqual(false, viewModel.ShareCommand.CanExecute());
        }

        [TestMethod]
        public void CanExecuteShareCommandIfTextCanBeChangedToValidUri()
        {
            var viewModel = CreateViewModel();

            viewModel.Text.Value = "test";

            Assert.AreEqual(true, viewModel.ShareCommand.CanExecute());
        }
    }
}