using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
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

        [TestInitialize]
        public void Initialize()
        {
            _windowService = A.Fake<IWindowService>();
            _dataTransferService = A.Fake<IDataTransferService>();
            _clipboardService = A.Fake<IClipboardService>();
            _httpService = A.Fake<IHttpService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        private MainPageViewModel CreateViewModel()
        {
            return new MainPageViewModel(_windowService, _dataTransferService, _clipboardService, _httpService);
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


    }
}