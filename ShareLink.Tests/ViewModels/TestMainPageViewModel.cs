using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Interfaces;
using ShareLink.Models;
using ShareLink.Services;
using ShareLink.Services.Interfaces;
using ShareLink.Tests.Mocks;
using ShareLink.ViewModels.ViewModels;
using Utilities.Functional;

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
        private ITextToSpeechService _textToSpeechService;
        private ApplicationSettingsService _applicationSettingsService;
        private ISettingsService _uiSettingsService;
        private IShareDataRepository _shareDataRepository;
        private INavigationService _navigationService;

        [TestInitialize]
        public void Initialize()
        {
            _windowService = A.Fake<IWindowService>();
            _dataTransferService = A.Fake<IDataTransferService>();
            _clipboardService = A.Fake<IClipboardService>();
            _httpService = A.Fake<IHttpService>();
            _schedulerProvider = A.Fake<ISchedulerProvider>();
            _textToSpeechService = A.Fake<ITextToSpeechService>();
            _applicationSettingsService = new ApplicationSettingsService(new MockApplicationDataContainer());
            _uiSettingsService = A.Fake<ISettingsService>();
            _shareDataRepository = A.Fake<IShareDataRepository>();
            _navigationService = A.Fake<INavigationService>(); 

            A.CallTo(() => _schedulerProvider.Default).Returns(_testScheduler);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        private MainPageViewModel CreateViewModel()
        {
            return new MainPageViewModel(_windowService, _dataTransferService, _clipboardService,
                _httpService, _schedulerProvider, _textToSpeechService, _applicationSettingsService,
                _uiSettingsService, _navigationService);
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
            A.CallTo(() => _clipboardService.GetTextAsync(A<CancellationToken>.Ignored)).Returns(Task.FromResult(Option<string>.AsOption(clipboardText)));
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
            _testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(300).Ticks);

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

        [TestMethod]
        public void CannotPressEnterCommandIfTextIsNotValidUri()
        {
            var viewModel = CreateViewModel();

            viewModel.Text.Value = "#$%^&*(";

            Assert.AreEqual(false, viewModel.KeyPressedCommand.CanExecute());
        }

        [TestMethod]
        public void CanPressEnterCommandIfTextCanBeChangedToValidUri()
        {
            var viewModel = CreateViewModel();
            viewModel.Text.Value = "test";

            Assert.AreEqual(true, viewModel.KeyPressedCommand.CanExecute());
        }

        [TestMethod]
        public void TriggerShareWhenExecutingShare()
        {
            var viewModel = CreateViewModel();
            viewModel.Text.Value = "test";

            viewModel.ShareCommand.Execute();

            A.CallTo(() => _dataTransferService.Share(A<ShareData>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        [Ignore] // verify title
        public void ResolveTitleOfPage()
        {
            const string pageTitle = "SomeTitle";
            A.CallTo(() => _httpService.GetPageTitleAsync(A<Uri>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(Option<string>.AsOption(pageTitle) ));
            var viewModel = CreateViewModel();
            viewModel.Text.Value = "test";

            viewModel.ShareCommand.Execute();

            A.CallTo(() => _dataTransferService.Share(A<ShareData>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void TappingSettingsButtonShowsSettings()
        {
            var viewModel = CreateViewModel();

            viewModel.SettingsCommand.Execute(null);

            A.CallTo(() => _uiSettingsService.ShowSettings()).MustHaveHappened(Repeated.Exactly.Once);
        }


        [TestMethod]
        public void StartingSharingStartsProgress()
        {
            var viewModel = CreateViewModel();
            viewModel.Text.Value = "test";
            A.CallTo(() => _httpService.GetPageTitleAsync(A<Uri>.Ignored, A<CancellationToken>.Ignored)).Returns(Task.FromResult(Option<string>.AsOption("pageTitle")));
            bool progressChangedToTrue = false;
            viewModel.IsInProgress.Subscribe(isInProgress => progressChangedToTrue |= isInProgress);

            viewModel.ShareCommand.Execute();

            Assert.AreEqual(true, progressChangedToTrue);
        }

    }
}