using Microsoft.Practices.Prism.Mvvm.Interfaces;
using ShareLink.Services.Interfaces;

namespace ShareLink.UiServices.WindowsPhone
{
    public class SettingService : ISettingsService
    {
        private readonly INavigationService _navigationService;

        public SettingService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void ShowSettings()
        {
            _navigationService.Navigate("Settings", null);
        }
    }
}