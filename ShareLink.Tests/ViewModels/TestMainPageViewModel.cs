using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Interfaces;
using ShareLink.ViewModels.ViewModels;

namespace ShareLink.Tests.ViewModels
{
    public interface ITest
    {
        int Test();
    }

    [TestClass]
    public class TestMainPageViewModel
    {
        private MainPageViewModel _viewModel;
        private IWindowService _windowService;
        private IDataTransferService _dataTransferService;
        private IClipboardService _clipboardService;

        [TestInitialize]
        public void Initialize()
        {
            _windowService = A.Fake<IWindowService>();
            _dataTransferService = A.Fake<IDataTransferService>();
            _clipboardService = A.Fake<IClipboardService>();
            _viewModel = new MainPageViewModel(_windowService, _dataTransferService, _clipboardService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _viewModel.Dispose();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var test = A.Fake<ITest>();

            A.CallTo(() => test.Test()).Returns(1);

            Assert.AreEqual(1, test.Test());
        }
    }
}