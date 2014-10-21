#if WINDOWS_APP
using ShareLink.Services.Interfaces;
using ShareLink.Views;

namespace ShareLink.UiServices.Windows
{
    public class SettingService : ISettingsService
    {
        public void ShowSettings()
        {

            new SettingsPage().Show();

        }
    }
}
#endif