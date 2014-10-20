using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareLink.Services;
using ShareLink.Tests.Mocks;
using ShareLink.ViewModels.ViewModels;

namespace ShareLink.Tests.ViewModels
{
    [TestClass]
    public class TestSettingsPageViewModel
    {
        private ApplicationSettingsService _applicationSettingsService;
        

        [TestInitialize]
        public void Initialize()
        {
            _applicationSettingsService = new ApplicationSettingsService(new MockApplicationDataContainer());
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        private SettingsPageViewModel CreateViewModel()
        {
            return new SettingsPageViewModel(_applicationSettingsService);
        }


        [TestMethod]
        public void ByDefaultSpeechIsDisabled()
        {
            Assert.AreEqual(false, _applicationSettingsService.IsSpeechEnabled);
        }

        [TestMethod]
        public void ByDefaultViewModelsSpeechIsDisabled()
        {
            var vm = CreateViewModel();

            Assert.AreEqual(false, vm.IsSpeechEnabled.Value);
        }

        [TestMethod]
        public void WhenSpeechChangesInSettingsItChangesInVm()
        {
            var vm = CreateViewModel();

            _applicationSettingsService.IsSpeechEnabled = true;

            Assert.AreEqual(true, vm.IsSpeechEnabled.Value);
        }

        [TestMethod]
        public void WhenSpeechChangesInVmItChangesSettings()
        {
            var vm = CreateViewModel();

            vm.IsSpeechEnabled.Value = true;

            Assert.AreEqual(true, _applicationSettingsService.IsSpeechEnabled);
        }
    }
}